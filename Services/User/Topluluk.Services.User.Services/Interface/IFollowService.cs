using Topluluk.Services.User.Model.Dto;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.Services.Interface;

public interface IFollowService
{
    
    Task<Response<NoContent>> FollowUser(string userId, UserFollowDto userFollowInfo);
    Task<Response<NoContent>> UnFollowUser(string userId, UserFollowDto userUnFollowInfo);
    Task<Response<NoContent>> AcceptFollowRequest(string id, string targetId);
    Task<Response<NoContent>> DeclineFollowRequest(string id, string targetId);
    Task<Response<NoContent>> RemoveFollowRequest(string userId, string targetId);
    
    Task<Response<string>> RemoveUserFromFollowers(string userId, UserFollowDto userInfo);

    Task<Response<List<FollowingUserDto>>> GetFollowingUsers(string id, string userId, int skip = 0, int take = 10);
    Task<Response<List<FollowerUserDto>>> GetFollowerUsers(string id, string userId, int skip = 0, int take = 10);
    
    // Use for show Incoming follow requests
    Task<Response<List<UserFollowRequestDto>>> GetFollowerRequests(string id, string userId, int skip = 0, int take = 10);

    Task<Response<List<UserSuggestionsDto>>> GetUserFollowSuggestions(string userId);
    
    
    
    
    //Http
    
    
    /// <summary>
    /// Kullanıcının postlarını getirmek için takip ettiği kişilerin tamamını döndürüyorum.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Response<List<string>>> GetUserFollowings(string id);


}