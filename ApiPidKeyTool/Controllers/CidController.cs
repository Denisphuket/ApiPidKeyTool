using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class CidController : ControllerBase
{
    private readonly CidService _cidService;

    public CidController(CidService cidService)
    {
        _cidService = cidService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCid([FromQuery] string installationId)
    {
        if (string.IsNullOrWhiteSpace(installationId))
        {
            return BadRequest("InstallationId is required");
        }

        try
        {
            var response = await _cidService.GetCidAsync(installationId);
            return Ok(response); // Возвращается XML ответ или его обработанный результат
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ошибка сервера: " + ex.Message);
        }
    }
}
