using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.FileAPI.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;
namespace Topluluk.Services.FileAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly ICapPublisher _publisher;

        public FileController(IStorageService storageService, ICapPublisher capPublisher)
        {
            _storageService = storageService;
            _publisher = capPublisher;
        }
        
        [HttpGet("[action]")]
        public async Task<IActionResult> Index(IFormFileCollection files)
        {
            return Ok("Çalışıyor");
        }
        [HttpPost("upload-community-cover")]
        public async Task<Response<string>> UploadCommunityCoverImage(IFormFile file)
        {
            var result = await _storageService.UploadOneAsync("community-cover-images", file);
            return await Task.FromResult(Response<string>.Success(result.Data, ResponseStatus.Success));
        }
        
        
        [HttpPost("[action]")]
        public async Task<Response<List<string>>> UploadUserImage(IFormFileCollection files)
        {
            var result =  await _storageService.UploadAsync("user-images", files);
            return await Task.FromResult(Response<List<string>>.Success(result, ResponseStatus.Success));
        }

        [HttpPost("upload-user-banner")]
        public async Task<Response<string>> UploadUserBannerImage(IFormFile file)
        {
            var result = await _storageService.UploadOneAsync("user-banner-images", file);
            return await Task.FromResult(Response<string>.Success(result.Data, ResponseStatus.Success));
        }

        [HttpPost("delete-user-banner")]
        public async Task<Response<string>> DeleteUserBannerImage([FromBody] string fileName)
        {
            return await _storageService.DeleteAsync("user-banner-images", fileName);
        }

        [HttpPost("upload-post-files")]
        public async Task<Response<List<string>>> UploadPostFiles(IFormFileCollection files)
        {
            var result = await _storageService.UploadAsync("post-files", files);
            return await Task.FromResult(Response<List<string>>.Success(result, ResponseStatus.Success));
        }
        
        [HttpPost("event-images")]
        public async Task<Response<List<string>>> UploadEventImages(IFormFileCollection files)
        {
            var result =  await _storageService.UploadAsync("event-images", files);
            return await Task.FromResult(Response<List<string>>.Success(result, ResponseStatus.Success));
        }
        [HttpPost("[action]")]
        public async Task<Response<string>> DeleteUserImage( [FromBody] string fileName)
        {
            return await _storageService.DeleteAsync("user-images", fileName);
        }
    }
}

