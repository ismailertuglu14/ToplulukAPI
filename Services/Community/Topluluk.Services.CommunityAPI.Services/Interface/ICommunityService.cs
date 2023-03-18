using System;
using Microsoft.AspNetCore.Http;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Dto.Http;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Services.Interface
{
	public interface ICommunityService
	{
		Task<Response<List<Community>>> GetCommunities();
		Task<Response<List<CommunityGetPreviewDto>>> GetCommunitySuggestions(string userId, HttpRequest request, int skip = 0, int take = 5 );
		Task<Response<CommunityGetByIdDto>> GetCommunityById(string userId, string id);
		Task<Response<string>> Join(CommunityJoinDto communityInfo);
		Task<Response<string>> Create(CommunityCreateDto communityInfo);
		Task<Response<string>> Leave();
		Task<Response<string>> Delete(string ownerId, string communityId);
        Task<Response<string>> DeletePermanently(string ownerId, string communityId);
		Task<Response<List<string>>> GetParticipiants(string id);
        Task<Response<string>> KickUser();
		Task<Response<string>> AcceptUserJoinRequest();
		Task<Response<string>> DeclineUserJoinRequest();
		Task<Response<string>> AssignUserAsAdmin(AssignUserAsAdminDto dtoInfo);
		Task<Response<string>> AssignUserAsModerator(AssignUserAsModeratorDto dtoInfo);
        Task<Response<string>> UpdateCoverImage(CommunityImageUploadedDto dto);
		//http
		Task<Response<List<CommunityGetPreviewDto>>> GetUserCommunities(string userId);
		Task<Response<string>> PostCreated(PostCreatedCommunityDto dto);
		Task<Response<string>> GetCommunityTitle(string id);
		Task<Response<bool>> CheckCommunityExist(string id);
    }
}

