using System;
using Microsoft.AspNetCore.Http;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Services.Interface
{
	public interface ICommunityService
	{
		Task<Response<List<Community>>> GetCommunities();
		Task<Response<List<object>>> GetCommunitySuggestions(int skip, int take, HttpRequest request);
		Task<Response<string>> Join(CommunityJoinDto communityInfo)
			;
		Task<Response<string>> Create(CommunityCreateDto communityInfo);
		Task<Response<string>> Leave();
		Task<Response<string>> Delete(string communityId);
		Task<Response<string>> KickUser();
		Task<Response<string>> AcceptUserJoinRequest();
		Task<Response<string>> DeclineUserJoinRequest();
		Task<Response<string>> AssignUserAsAdmin(AssignUserAsAdminDto dtoInfo);
		Task<Response<string>> AssignUserAsModerator(AssignUserAsModeratorDto dtoInfo);
        Task<Response<string>> UpdateCoverImage(CommunityImageUploadedDto dto);
    }
}

