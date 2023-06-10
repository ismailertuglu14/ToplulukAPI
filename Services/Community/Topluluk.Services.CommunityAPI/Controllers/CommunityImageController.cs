using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Services.Interface;
using Topluluk.Services.FileAPI.Model.Dto.Http;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Controllers;

[Route("Community")]
public class CommunityImageController : BaseController
{
    private readonly ICommunityImageService _communityService;

    public CommunityImageController(ICommunityImageService communityService)
    {
        _communityService = communityService;
    }


    [HttpPost("{communityId}/update-cover-image")]
    public async Task<Response<string>> UpdateCoverImage(string communityId, [FromForm] CoverImageUpdateDto dto)
    {
        return await _communityService.UpdateCoverImage(this.UserId,communityId, dto);
    }

    [HttpPost("{communityId}/update-banner-image")]
    public async Task<Response<string>> UpdateBannerImage(string communityId, [FromForm] BannerImageUpdateDto dto)
    {
        return await _communityService.UpdateBannerImage(this.UserId,communityId, dto);
    }
    [HttpPost("{communityId}/remove-banner-image")]
    public async Task<Response<NoContent>> RemoveBannerImage(string communityId)
    {
        return await _communityService.RemoveBannerImage(this.UserId,communityId);
    }
    [HttpPost("{communityId}/remove-cover-image")]
    public async Task<Response<NoContent>> RemoveCoverImage(string communityId)
    {
        return await _communityService.RemoveCoverImage(this.UserId,communityId);
    }
}