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

		Task<Response<string>> GetUserSuggestions(int limit = 4);

		Task<Response<string>> InsertUser(UserInsertDto userInfo);

		Task<Response<string>> DeleteUserById(string id);

		Task<Response<string>> FollowUser(UserFollowDto userFollowInfo);
		Task<Response<string>> UnFollowUser(UserFollowDto userUnFollowInfo);
		Task<Response<string>> RemoveUserFromFollowers(UserFollowDto userInfo);

		Task<Response<string>> BlockUser(string sourceId, string targetId);

		Task<Response<string>> SearchUser(string text, int skip = 0, int take = 5);

		Task<Response<string>> ChangeProfileImage(string userName, IFormFileCollection files);

		// Http calls services
		Task<Response<string>> UpdateCommunities(string userId, string communityId);
    }
}

