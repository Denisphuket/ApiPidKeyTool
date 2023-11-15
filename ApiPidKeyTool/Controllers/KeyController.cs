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

    [HttpGet]
    public async Task<IActionResult> CheckKeys([FromQuery] string keys)
    {
        try
        {
            var responses = await _keyService.CheckKeysAsync(keys);
            return Ok(responses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
