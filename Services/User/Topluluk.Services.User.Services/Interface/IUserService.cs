using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.Services.Interface
{
	public interface IUserService
	{
		Task<Response<string>> GetUserById(string id);
		Task<Response<string>> GetUserByUserName(string userName);

        Task<Response<List<UserSuggestionsDto>>> GetUserSuggestions(string userId, int limit = 5);
		Task<Response<List<UserSuggestionsDto>>> GetUserSuggestionsMore(int skip = 0, int take = 5);
		
        Task<Response<string>> InsertUser(UserInsertDto userInfo);

		Task<Response<string>> DeleteUserById(string id);

		Task<Response<string>> FollowUser(UserFollowDto userFollowInfo);
		Task<Response<string>> UnFollowUser(UserFollowDto userUnFollowInfo);
		Task<Response<string>> RemoveUserFromFollowers(UserFollowDto userInfo);

		Task<Response<string>> BlockUser(string sourceId, string targetId);

		Task<Response<string>> SearchUser(string text, int skip = 0, int take = 5);

		Task<Response<string>> ChangeProfileImage(string userName, IFormFileCollection files);
		Task<Response<string>> ChangeBannerImage(string userId, IFormFile file);

        // Http calls services
        Task<Response<string>> UpdateCommunities(string userId, string communityId);
    }
}

