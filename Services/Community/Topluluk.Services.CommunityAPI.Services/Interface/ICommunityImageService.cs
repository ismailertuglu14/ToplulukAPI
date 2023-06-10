using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.FileAPI.Model.Dto.Http;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Services.Interface;

public interface ICommunityImageService
{
    Task<Response<string>> UpdateCoverImage(string userId, string communityId, CoverImageUpdateDto dto);
    Task<Response<string>> UpdateBannerImage(string userId, string communityId, BannerImageUpdateDto dto);
    Task<Response<NoContent>> RemoveCoverImage(string userId, string communityId);
    Task<Response<NoContent>> RemoveBannerImage(string userId, string communityId);

}