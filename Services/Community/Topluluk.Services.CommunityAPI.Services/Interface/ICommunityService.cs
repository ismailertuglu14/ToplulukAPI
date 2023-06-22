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

		Task<Response<List<CommunityGetPreviewDto>>> GetCommunitySuggestions(string userId, HttpRequest request, int skip = 0, int take = 5 );
	
		// Community Detail Page
		Task<Response<CommunityGetByIdDto>> GetCommunityById(string userId,string token, string id);
		Task<Response<string>> Create(string userId, string token, CommunityCreateDto communityInfo);
		Task<Response<string>> Delete(string ownerId, string communityId);
        Task<Response<string>> DeletePermanently(string ownerId, string communityId);
        
      
        Task<Response<NoContent>> KickUser(string token, string communityId, string userId);
	
		Task<Response<string>> AssignUserAsAdmin(string userId, AssignUserAsAdminDto dtoInfo);
		Task<Response<string>> AssignUserAsModerator(AssignUserAsModeratorDto dtoInfo);
		Task<Response<List<CommunityGetPreviewDto>>> GetUserCommunities( string userId, int skip = 0, int take = 10);
		
        Task<Response<int>> GetUserParticipiantCommunitiesCount(string userId);
        Task<Response<string>> GetCommunityTitle(string id);

		Task<Response<bool>> CheckCommunityExist(string id);
		Task<Response<bool>> CheckIsUserAdminOwner(string userId);

        Task<Response<CommunityInfoPostLinkDto>> GetCommunityInfoForPostLink(string id);

    }
}

