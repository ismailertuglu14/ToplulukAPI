using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.FileAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.FileAPI.Controllers;

[ApiController]
[Route("file")]
public class UserFileController
{
    private readonly IStorageService _storageService;

    public UserFileController(IStorageService storageService)
    {
        _storageService = storageService;
    }
    [HttpPost("upload-user-image")]
    public async Task<Response<List<string>>> UploadUserImage(IFormFileCollection files)
    {
        var result =  await _storageService.UploadAsync("user-images", files);
        return await Task.FromResult(Response<List<string>>.Success(result, ResponseStatus.Success));
    }

    [HttpPost("delete-user-image")]
    public async Task<Response<string>> DeleteUserImage( NameObject fileName)
    {
        var result = await _storageService.DeleteAsync("user-images", fileName.Name);
        return  Response<string>.Success(result.Data, ResponseStatus.Success);
    }
    
    [HttpPost("upload-user-banner")]
    public async Task<Response<string>> UploadUserBannerImage(IFormFile file)
    {
        var result = await _storageService.UploadOneAsync("user-banner-images", file);
        return await Task.FromResult(Response<string>.Success(result.Data, ResponseStatus.Success));
    }

    [HttpPost("delete-user-banner")]
    public async Task<Response<string>> DeleteUserBannerImage(NameObject fileName)
    {
        return await _storageService.DeleteAsync("user-banner-images", fileName.Name);
    }
}