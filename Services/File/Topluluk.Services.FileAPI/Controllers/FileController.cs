using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.FileAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Topluluk.Services.FileAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public FileController(IStorageService storageService)
        {
            _storageService = storageService;
        }
        
        // GET: /<controller>/
        [HttpGet("[action]")]
        public async Task<IActionResult> Index(IFormFileCollection files)
        {
            return Ok("Çalışıyor");
        }

        // POST:
        [HttpPost("[action]")]
        public async Task<Response<List<string>>> UploadUserImage(IFormFileCollection files)
        {
            var result =  await _storageService.UploadAsync("user-images", files);
            return await Task.FromResult(Response<List<string>>.Success(result, ResponseStatus.Success));
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> DeleteUserImage( [FromBody] string fileName)
        {
            return await _storageService.DeleteAsync("user-images", fileName);
        }
           
    }
}

