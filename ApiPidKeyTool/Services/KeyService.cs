using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

public class KeyService
{
    public async Task<List<KeyResponse>> CheckKeysAsync(string keys)
    {
        string exePath = Path.Combine(Path.GetTempPath(), "PidKey.exe");

        // Извлечение PidKey.exe из ресурсов и сохранение во временном файле
        byte[] exeBytes = ApiPidKeyTool.Properties.Resources.PidKey;
        File.WriteAllBytes(exePath, exeBytes);

        string output = "";

        using (Process process = new Process())
        {
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {exePath} /CheckKey {keys}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process.Start();
            output = await process.StandardOutput.ReadToEndAsync();
            string errorOutput = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(errorOutput))
            {
                throw new Exception("Ошибка внешнего процесса: " + errorOutput);
            }
        }

        // Удаление временного файла PidKey.exe после использования
        File.Delete(exePath);

        return ParseKeyOutput(output);
    }

    private List<KeyResponse> ParseKeyOutput(string output)
    {
        var responses = new List<KeyResponse>();
        var jsonResponses = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var jsonResponse in jsonResponses)
        {
            try
            {
                var response = JsonSerializer.Deserialize<KeyResponse>(jsonResponse);
                if (response != null)
                {
                    responses.Add(response);
                }
                else
                {
                    // Добавление объекта с пустыми/умолчательными значениями
                    responses.Add(new KeyResponse
                    {
                        Key = "",
                        Description = "",
                        Remain = "0",
                        ErrorCode = "Не удалось обработать ключ",
                        IID = ""
                    });
                }
            }
            catch (JsonException ex)
            {
                // Обработка ошибок парсинга JSON
                Console.WriteLine($"Ошибка парсинга: {ex}");
            }
        }

        return responses;
    }
}
