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
		Task<Response<List<UserFollowerRequestDto>>> GetFollowerRequests(string userId, int skip = 0, int take = 10);

		Task<Response<string>> BlockUser(string sourceId, string targetId);

		Task<Response<List<UserSearchResponseDto>>?> SearchUser(string text, string userId, int skip = 0, int take = 5);

		Task<Response<string>> ChangeProfileImage(string userName, IFormFileCollection files);
		Task<Response<string>> ChangeBannerImage(UserChangeBannerDto changeBannerDto);

		Task<Response<string>> PrivacyChange(string userId, UserPrivacyChangeDto dto);
		
        // Http calls services
        Task<Response<string>> UpdateCommunities(string userId, string communityId);
		Task UserBanngerChanged(string userId, string fileName);
		Task<Response<string>> PostCreated(string userId,string id);
		Task<Response<UserInfoGetResponse>> GetUserInfoForPost(string id, string sourceUserId);
		Task<Response<GetCommunityOwnerDto>> GetCommunityOwner(string id);
		Task<Response<UserInfoForCommentDto>> GetUserInfoForComment(string id);
        Task<Response<List<string>>> GetUserFollowings(string id);
		Task<Response<List<GetUserByIdDto>>> GetUserList(UserIdListDto dto, int skip = 0, int take = 10);
    }

}

