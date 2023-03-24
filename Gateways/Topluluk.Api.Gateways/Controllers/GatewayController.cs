using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Topluluk.Api.Gateways.Controllers;

[EnableCors("MyPolicy")]
public class GatewayController : ControllerBase
{
    // GET
    [HttpGet("[action]")]
    public IActionResult Index()
    {
        return Ok("Gateway controller works fine");
    }
}