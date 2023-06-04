using System;
using Microsoft.AspNetCore.Http;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Dto.Http;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Services.FileAPI.Model.Dto.Http;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Services.Interface
{
	public interface ICommunityService
	{

		Task<Response<List<Community>>> GetCommunities();
		Task<Response<List<CommunityGetPreviewDto>>> GetCommunitySuggestions(string userId, HttpRequest request, int skip = 0, int take = 5 );
	
		// Community Detail Page
		Task<Response<CommunityGetByIdDto>> GetCommunityById(string userId,string token, string id);
		Task<Response<string>> Join(string userId, string token, string communityId);
		Task<Response<string>> Create(string userId, string token, CommunityCreateDto communityInfo);
		Task<Response<NoContent>> Leave(string userId, string token, string communityId);
		Task<Response<string>> Delete(string ownerId, string communityId);
        Task<Response<string>> DeletePermanently(string ownerId, string communityId);
		Task<Response<List<UserDto>>> GetParticipiants(string token, string id);
        Task<Response<NoContent>> KickUser(string token, string communityId, string userId);
		Task<Response<string>> AcceptUserJoinRequest();
		Task<Response<string>> DeclineUserJoinRequest();
		Task<Response<string>> AssignUserAsAdmin(string userId, AssignUserAsAdminDto dtoInfo);
		Task<Response<string>> AssignUserAsModerator(AssignUserAsModeratorDto dtoInfo);

        Task<Response<string>> UpdateCoverImage(string userId, string communityId, CoverImageUpdateDto dto);
        Task<Response<NoContent>> UpdateBannerImage(string userId, string communityId, BannerImageUpdateDto dto);
 
        /// Lists the communities that the user is a member of in the profile
        Task<Response<List<CommunityGetPreviewDto>>> ParticipiantCommunities(string sourceId, string targetId);
		//http
		Task<Response<List<CommunityGetPreviewDto>>> GetUserCommunities( string userId);
		
        // Use this function for select community while createing a new post.
        Task<Response<List<CommunityInfoPostLinkDto>>> GetParticpiantsCommunities(string userId, string token);
        Task<Response<int>> GetUserParticipiantCommunitiesCount(string userId);
        Task<Response<string>> GetCommunityTitle(string id);

		Task<Response<bool>> CheckCommunityExist(string id);
		Task<Response<bool>> CheckIsUserAdminOwner(string userId);

        Task<Response<CommunityInfoPostLinkDto>> GetCommunityInfoForPostLink(string id);

		Task<Response<bool>> LeaveUserDelete(string id, IdList list);
    }
}

