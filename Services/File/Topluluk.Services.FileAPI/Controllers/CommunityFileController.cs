using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.FileAPI.Services.Interface;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.FileAPI.Controllers;

[ApiController]
[Route("file")]
public class CommunityFileController
{
    
    
    private readonly IStorageService _storageService;

    public CommunityFileController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost("upload-community-cover-image")]
    public async Task<Response<string>> UploadCommunityCoverImage(IFormFile file)
    {
        return await _storageService.UploadOneAsync("community-cover-images",file);
    }
    
    [HttpPost("delete-community-cover-image")]
    public async Task<Response<string>> DeleteCommunityCoverImage(NameObject fileName)
    {
        return await _storageService.DeleteAsync("community-cover-images",fileName.Name);
    }
    
    [HttpPost("upload-community-banner-image")]
    public async Task<Response<string>> UploadCommunityBannerImage(IFormFile file)
    {
        return await _storageService.UploadOneAsync("community-cover-images",file);
    }
    
    [HttpPost("delete-community-banner-image")]
    public async Task<Response<string>> DeleteCommunityBannerImage(NameObject fileName)
    {
        return await _storageService.DeleteAsync("community-cover-images",fileName.Name);
    }
}