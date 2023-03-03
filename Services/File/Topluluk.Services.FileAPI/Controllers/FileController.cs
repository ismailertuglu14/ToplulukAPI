using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.FileAPI.Services.Interface;
using Topluluk.Shared.Constants;
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
        private readonly ICapPublisher _publisher;

        public FileController(IStorageService storageService, ICapPublisher capPublisher)
        {
            _storageService = storageService;
            _publisher = capPublisher;
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

        [NonAction]
        [CapSubscribe(QueueConstants.COMMUNITY_IMAGE_UPLOAD)]
        public async Task<Response<List<string>>> ImageUpload(CommunityImageUploadDto dto)
        {
            using var stream = new MemoryStream(dto.CoverImage);
            var formFile = new FormFile(stream, 0, stream.Length, null, dto.FileName);
            var files = new FormFileCollection();
            files.Add(formFile);
            var result = await _storageService.UploadAsync("community-images", files);
            var imageUrl = result[0];
            await _publisher.PublishAsync(QueueConstants.COMMUNITY_IMAGE_UPLOADED, new { CommunityId= dto.CommunityId, CoverImage = imageUrl });
            return await Task.FromResult(Response<List<string>>.Success(result, ResponseStatus.Success));

        }

        [NonAction]
        [CapSubscribe(QueueConstants.USER_CHANGE_IMAGE)]
        public async Task<Response<List<string>>> UserChangeImage()
        {
            return new();
        }
    }

    public class CommunityImageUploadDto
    {
        public string CommunityId { get; set; }
        public string FileName { get; set; }
        public byte[]? CoverImage { get; set; }
    }
}

