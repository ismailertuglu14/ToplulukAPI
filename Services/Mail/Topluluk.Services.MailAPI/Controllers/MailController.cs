using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Shared;
using Topluluk.Shared.BaseModels;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Topluluk.Services.MailAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailController : BaseController
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return Ok("");
        }

    }
}

