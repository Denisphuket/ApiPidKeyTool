using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class KeyController : ControllerBase
{
    private readonly KeyService _keyService;

    public KeyController(KeyService keyService)
    {
        _keyService = keyService;
    }

    [HttpPost]
    public async Task<IActionResult> CheckKey([FromBody] KeyRequest keyRequest)
    {
        try
        {
            var result = await _keyService.CheckKeyAsync(keyRequest.Key);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
