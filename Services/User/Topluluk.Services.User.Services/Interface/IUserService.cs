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
		Task<Response<GetUserByIdDto>> GetUserByUserName(string id, string userName);
        Task<Response<List<UserSuggestionsDto>>> GetUserSuggestions(string userId, int limit = 5);
		Task<Response<List<UserSuggestionsDto>>> GetUserSuggestionsMore(int skip = 0, int take = 5);
		
        Task<Response<string>> InsertUser(UserInsertDto userInfo);

		Task<Response<string>> DeleteUserById(string id, string token, UserDeleteDto userInfo);

		Task<Response<string>> FollowUser(string userId, UserFollowDto userFollowInfo);
		Task<Response<string>> UnFollowUser(string userId, UserFollowDto userUnFollowInfo);
		Task<Response<NoContent>> RemoveFollowRequest(string userId, string targetId);
		Task<Response<string>> AcceptFollowRequest(string id, string targetId);
		
        Task<Response<string>> DeclineFollowRequest(string id, string targetId);
        Task<Response<string>> RemoveUserFromFollowers(string userId, UserFollowDto userInfo);

		Task<Response<List<FollowingUserDto>>> GetFollowingUsers(string id, string userId, int skip = 0, int take = 10);
		Task<Response<List<FollowerUserDto>>> GetFollowerUsers(string id, string userId, int skip = 0, int take = 10);
		// Use for show Incoming follow requests
		Task<Response<List<UserFollowRequestDto>>> GetFollowerRequests(string id, string userId, int skip = 0, int take = 10);


		Task<Response<string>> BlockUser(string sourceId, string targetId);

		Task<Response<List<UserSearchResponseDto>>?> SearchUser(string text, string userId, int skip = 0, int take = 5);
		Task<Response<List<FollowingUserDto>>> SearchInFollowings(string id, string userId, string text, int skip = 0, int take = 10);

		Task<Response<string>> ChangeProfileImage(string userName, IFormFileCollection files, CancellationToken cancellationToken);
		Task<Response<NoContent>> DeleteProfileImage(string userId);
		Task<Response<string>> ChangeBannerImage(string userId, UserChangeBannerDto changeBannerDto);
		Task<Response<NoContent>> DeleteBannerImage(string userId);

		Task<Response<string>> PrivacyChange(string userId, UserPrivacyChangeDto dto);

		Task<Response<NoContent>> UpdateProfile(string userId, string token, UserUpdateProfileDto userDto);

        // Http calls services
        // I'm using for get UserId, UserName, firstName, lastName, ProfileImage
         Task<Response<UserInfoForPostDto>> GetUserInfoForPost(string id, string sourceUserId);
		Task<Response<GetCommunityOwnerDto>> GetCommunityOwner(string id);
		Task<Response<UserInfoForCommentDto>> GetUserInfoForComment(string id);
        Task<Response<List<string>>> GetUserFollowings(string id);

		// If you have a list of user id's and u need to get user by id, Then use this function
		Task<Response<List<GetUserByIdDto>>> GetUserList(IdList dto, int skip = 0, int take = 10);
    }

}

