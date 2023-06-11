using Topluluk.Services.User.Model.Dto;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.Services.Interface;

public interface IFollowService
{
    
    Task<Response<NoContent>> FollowUser(string userId, UserFollowDto userFollowInfo);
    Task<Response<NoContent>> UnFollowUser(string userId, UserFollowDto userUnFollowInfo);
    
    
    /// <summary>
    /// It is used if the user wants to remove a follow request made to another user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="targetId"></param>
    /// <returns></returns>
    Task<Response<NoContent>> RemoveFollowRequest(string userId, string targetId);
    
            
    /// <summary>
    /// It is used to accept follow requests from the other users.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="targetId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    Task<Response<NoContent>> AcceptFollowRequest(string id, string targetId);
		
    Task<Response<NoContent>> DeclineFollowRequest(string id, string targetId);
    Task<Response<string>> RemoveUserFromFollowers(string userId, UserFollowDto userInfo);

    Task<Response<List<FollowingUserDto>>> GetFollowingUsers(string id, string userId, int skip = 0, int take = 10);
    Task<Response<List<FollowerUserDto>>> GetFollowerUsers(string id, string userId, int skip = 0, int take = 10);
    
    // Use for show Incoming follow requests
    Task<Response<List<UserFollowRequestDto>>> GetFollowerRequests(string id, string userId, int skip = 0, int take = 10);
    
    
    //Http
    Task<Response<List<string>>> GetUserFollowings(string id);


}