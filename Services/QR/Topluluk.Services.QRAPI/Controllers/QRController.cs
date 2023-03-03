using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.QRAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Topluluk.Services.QRAPI.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class QRController : BaseController
    {
        private readonly IQRService _qrService;

        public QRController(IQRService qrService)
        {
            _qrService = qrService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateQRCode(string text)
        {
            var data = await _qrService.GenerateQRCodeAsync(text);
            return File(data.Data, "image/png");
        }
    }
}

