using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Shared.BaseModels;

namespace Topluluk.Api.Gateways.Controllers;

public class GatewayController : BaseController
{
    // GET
    [HttpGet("[action]")]
    public IActionResult Index()
    {
        return Ok("Gateway controller works fine");
    }
}