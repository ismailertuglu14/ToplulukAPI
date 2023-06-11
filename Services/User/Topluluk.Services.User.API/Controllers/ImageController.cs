using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Services.User.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.API.Controllers;

[ApiController]
[Route("User")]
public class ImageController:BaseController
{

    private IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }
    
    [HttpPost("[action]")]
    public async Task<Response<string>> ChangeProfileImage(IFormFileCollection files, CancellationToken cancellationToken)
    {
        return await _imageService.ChangeProfileImage(UserName, files,cancellationToken);
    }

    [HttpPost("remove-profile-image")]
    public async Task<Response<NoContent>> RemoveProfileImage()
    {
        return await _imageService.DeleteProfileImage(this.UserId);
    }
    [HttpPost("remove-banner-image")]
    public async Task<Response<NoContent>> RemoveBannerImage()
    {
        return await _imageService.DeleteBannerImage(this.UserId);
    }
    [HttpPost("[action]")]
    public async Task<Response<string>> ChangeBannerImage([FromForm] UserChangeBannerDto changeBannerDto)
    {
        return await _imageService.ChangeBannerImage(this.UserId, changeBannerDto);
    }
}