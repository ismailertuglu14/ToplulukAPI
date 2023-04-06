using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Services.User.Model.Dto.Http;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.Services.Interface
{
	public interface IUserService
	{
		Task<Response<GetUserByIdDto>> GetUserById(string id, string userId);
		Task<Response<GetUserAfterLoginDto>> GetUserAfterLogin(string id);
		Task<Response<GetUserByIdDto>> GetUserByUserName(string userName);
        Task<Response<List<UserSuggestionsDto>>> GetUserSuggestions(string userId, int limit = 5);
		Task<Response<List<UserSuggestionsDto>>> GetUserSuggestionsMore(int skip = 0, int take = 5);
		
        Task<Response<string>> InsertUser(UserInsertDto userInfo);

		Task<Response<string>> DeleteUserById(string id, string token, UserDeleteDto userInfo);

		Task<Response<string>> FollowUser(UserFollowDto userFollowInfo);
		Task<Response<string>> UnFollowUser(UserFollowDto userUnFollowInfo);
		Task<Response<string>> AcceptFollowRequest(string id, string targetId);
        Task<Response<string>> DeclineFollowRequest(string id, string targetId);
        Task<Response<string>> RemoveUserFromFollowers(UserFollowDto userInfo);

		Task<Response<List<FollowingUserDto>>> GetFollowingUsers(string userId, int skip = 0, int take = 10);
		Task<Response<List<FollowerUserDto>>> GetFollowerUsers(string userId, int skip = 0, int take = 10);
		Task<Response<List<FollowingRequestDto>>> GetFollowerRequests(string userId, int skip = 0, int take = 10);


		Task<Response<string>> BlockUser(string sourceId, string targetId);

		Task<Response<List<UserSearchResponseDto>>?> SearchUser(string text, string userId, int skip = 0, int take = 5);
		Task<Response<List<FollowingUserDto>>> SearchInFollowings(string id, string userId, string text, int skip = 0, int take = 10);

		Task<Response<string>> ChangeProfileImage(string userName, IFormFileCollection files);
		Task<Response<string>> ChangeBannerImage(UserChangeBannerDto changeBannerDto);

		Task<Response<string>> PrivacyChange(string userId, UserPrivacyChangeDto dto);

		Task<Response<NoContent>> UpdateProfile(string userId, string token, UserUpdateProfileDto userDto);

        // Http calls services
		Task<Response<NoContent>> JoinCommunity(string userId, string communityId);
		Task<Response<NoContent>> LeaveCommunity(string userId, string communityId);
        Task UserBanngerChanged(string userId, string fileName);
		Task<Response<UserInfoGetResponse>> GetUserInfoForPost(string id, string sourceUserId);
		Task<Response<GetCommunityOwnerDto>> GetCommunityOwner(string id);
		Task<Response<UserInfoForCommentDto>> GetUserInfoForComment(string id);
        Task<Response<List<string>>> GetUserFollowings(string id);

		// If you have a list of user id's and u need to get user by id, Then use this function
		Task<Response<List<GetUserByIdDto>>> GetUserList(UserIdListDto dto, int skip = 0, int take = 10);
    }

}

