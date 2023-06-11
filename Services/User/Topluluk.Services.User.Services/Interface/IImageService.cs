using Microsoft.AspNetCore.Http;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.Services.Interface;

public interface IImageService
{
    Task<Response<string>> ChangeProfileImage(string userName, IFormFileCollection files, CancellationToken cancellationToken);
    Task<Response<NoContent>> DeleteProfileImage(string userId);
    
    Task<Response<string>> ChangeBannerImage(string userId, UserChangeBannerDto changeBannerDto);
    Task<Response<NoContent>> DeleteBannerImage(string userId);
}