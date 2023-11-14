using System.Diagnostics;
using System.Threading.Tasks;

public class KeyService
{
    public async Task<string> CheckKeyAsync(string key)
    {
        string exePath = "PidKey.exe"; // Путь к PidKey.exe
        string output = "";

        using (Process process = new Process())
        {
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {exePath} /CheckKey {key}",
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

        return output;
    }
}
