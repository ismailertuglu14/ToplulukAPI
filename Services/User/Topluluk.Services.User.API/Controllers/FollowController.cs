using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Services.User.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.User.API.Controllers;


[ApiController]
[Route("User")]
public class FollowController : BaseController
{

    private readonly IFollowService _followService;
    
    public FollowController(IFollowService followService)
    {
        _followService = followService;
    }
    
    
    [HttpPost("Follow")]
    public async Task<Response<NoContent>> FollowUser( [FromBody] UserFollowDto userFollowInfo)
    {
        return await _followService.FollowUser(this.UserId, userFollowInfo);
    }

    [HttpPost("UnFollow")]
    public async Task<Response<NoContent>> UnFollowUser([FromBody] UserFollowDto userFollowInfo)
    {
        return await _followService.UnFollowUser(this.UserId, userFollowInfo);
    }
    [HttpPost("remove-follow-request/{targetId}")]
    public async Task<Response<NoContent>> RemoveFollowRequest(string targetId)
    {
        return await _followService.RemoveFollowRequest(this.UserId, targetId);
    }
    [HttpPost("remove-follower")]
    public async Task<Response<string>> RemoveUserFromFollower([FromBody] UserFollowDto userFollowInfo)
    {
        return await _followService.RemoveUserFromFollowers(this.UserId, userFollowInfo);
    }
    [HttpPost("accept-follow-request/{targetId}")]
    public async Task<Response<NoContent>> AcceptFollowRequest(string targetId)
    {
        return await _followService.AcceptFollowRequest(this.UserId, targetId);
    }
    [HttpPost("decline-follow-request/{targetId}")]
    public async Task<Response<NoContent>> DeclineFollowRequest(string targetId)
    {
        return await _followService.DeclineFollowRequest(this.UserId, targetId);
    }
    [HttpGet("incoming-requests/{id}")]
    public async Task<Response<List<UserFollowRequestDto>>> IncomingRequests(string id,int take, int skip)
    {
        return await _followService.GetFollowerRequests(this.UserId, id,skip,take);
    }
    [HttpGet("followings")]
    public async Task<Response<List<FollowingUserDto>>> GetFollowingUsers(string id, int take, int skip)
    {
        return await _followService.GetFollowingUsers(this.UserId, id, skip, take);
    }
    [HttpGet("followers")]
    public async Task<Response<List<FollowerUserDto>>> GetFollowerUsers(string id, int take, int skip)
    {
        return await _followService.GetFollowerUsers(this.UserId, id, skip, take);
    }
    [HttpGet("follow-suggestions")]
    public async Task<Response<List<UserSuggestionsDto>>> FollowSuggestions()
    {
        return await _followService.GetUserFollowSuggestions(this.UserId);
    }

    
    //HTTP
    [HttpGet("user-followings")]
    public async Task<Response<List<string>>> GetUserFollowings(string id)
    {
        return await _followService.GetUserFollowings(id);
    }

}