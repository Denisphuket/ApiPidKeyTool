using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Linq;

public class CidService
{
    private readonly HttpClient _httpClient;

    public CidService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetCidAsync(string installationId)
    {
        string extendedProductId = "03612-03308-019-582907-00-1049-19044.0000-3152023";
        string soapRequest = CreateSoapRequest(installationId, extendedProductId);

        var requestContent = new StringContent(soapRequest, Encoding.UTF8, "text/xml");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://activation.sls.microsoft.com/BatchActivation/BatchActivation.asmx")
        {
            Content = requestContent,
            Headers = 
            {
                { "SOAPAction", "http://www.microsoft.com/BatchActivationService/BatchActivate" }
            }
        };

        var response = await _httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();
        return ParseResponse(responseContent); // Здесь может быть дополнительная обработка XML ответа
    }

    private string ParseResponse(string response)
    {
        var soapEnvelope = XElement.Parse(response);
        var responseXml = soapEnvelope.Descendants().FirstOrDefault(x => x.Name.LocalName == "ResponseXml")?.Value;

        if (string.IsNullOrEmpty(responseXml))
        {
            return "Ответ не содержит данных";
        }

        var activationResponse = XElement.Parse(responseXml);
        var errorElement = activationResponse.Descendants().FirstOrDefault(x => x.Name.LocalName == "ErrorInfo");

        if (errorElement != null)
        {
            Console.WriteLine($"Error Element Found: {errorElement}");
            string errorCode = errorElement.Element("ErrorCode")?.Value;
            Console.WriteLine($"Error errorCode Found: {errorCode}");
            return ErrorCodeToMessage(errorCode);
        }

        var cidElement = activationResponse.Descendants().FirstOrDefault(x => x.Name.LocalName == "CID");
        return cidElement?.Value ?? "CID не найден";
    }

    private string ErrorCodeToMessage(string errorCode)
    {
        return errorCode switch
        {
            "0x7F" => "Необходимо звонить",
            "0x67" => "Ключ заблокирован",
            "0x8D" => "Превышено количество IID",
            "0x90" => "Неверный IID",
            "0x68" => "Ключ не легитимен",
            "0xD5" => "IID заблокирован",
            "0xD6" => "IID заблокирован",
            "0x86" => "-1",
            "0x71" => "0x71",
            "0x8E" => "0x71",
            "0xC004C017" => "0xC004C017",
            "0x80131509" => "0xC004C017",
            _ => $"Неизвестная ошибка: {errorCode}"
        };
    }

    private string CreateSoapRequest(string installationId, string extendedProductId)
    {
        byte[] privateKey = new byte[] { 0xfe, 0x31, 0x98, 0x75, 0xfb, 0x48, 0x84, 0x86, 0x9c, 0xf3, 0xf1, 0xce, 0x99, 0xa8, 0x90, 0x64, 0xab, 0x57, 0x1f, 0xca, 0x47, 0x04, 0x50, 0x58, 0x30, 0x24, 0xe2, 0x14, 0x62, 0x87, 0x79, 0xa0 };
        string xmlBody = $"<ActivationRequest xmlns=\"http://www.microsoft.com/DRM/SL/BatchActivationRequest/1.0\"><VersionNumber>2.0</VersionNumber><RequestType>1</RequestType><Requests><Request><PID>{extendedProductId}</PID><IID>{installationId}</IID></Request></Requests></ActivationRequest>";
        byte[] xmlBytes = Encoding.Unicode.GetBytes(xmlBody);
        string base64Xml = Convert.ToBase64String(xmlBytes);

        using var hmac = new HMACSHA256(privateKey);
        string digest = Convert.ToBase64String(hmac.ComputeHash(xmlBytes));

        return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
               "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n" +
               "    <soap:Body>\n" +
               "        <BatchActivate xmlns=\"http://www.microsoft.com/BatchActivationService\">\n" +
               "            <request>\n" +
               $"                <Digest>{digest}</Digest>\n" +
               $"                <RequestXml>{base64Xml}</RequestXml>\n" +
               "            </request>\n" +
               "        </BatchActivate>\n" +
               "    </soap:Body>\n" +
               "</soap:Envelope>";
    }
}
