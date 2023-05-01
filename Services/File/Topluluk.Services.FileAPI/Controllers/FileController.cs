using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.FileAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;
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
        
        [HttpPost("upload-community-cover")]
        public async Task<Response<string>> UploadCommunityCoverImage(IFormFile file)
        {
            var result = await _storageService.UploadOneAsync("community-cover-images", file);
            return await Task.FromResult(Response<string>.Success(result.Data, ResponseStatus.Success));
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
       
    }
}

