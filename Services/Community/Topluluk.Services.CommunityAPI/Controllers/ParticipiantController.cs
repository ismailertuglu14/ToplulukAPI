using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.CommunityAPI.Model.Dto.Http;
using Topluluk.Services.CommunityAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Controllers;

[Route("Community")]
public class ParticipiantController : BaseController
{
    private readonly IParticipiantService _participiantService;
    
    public ParticipiantController(IParticipiantService participiantService)
    {
        _participiantService = participiantService;
    }

    [HttpPost("{communityId}/join")]
    public async Task<Response<string>> Join(string communityId)
    {
        return await _participiantService.Join(this.UserId, this.Token, communityId);
    }
    
    [HttpPost("{communityId}/leave")]
    public async Task<Response<NoContent>> Leave(string communityId)
    {
        return await _participiantService.Leave(this.UserId, this.Token, communityId);
    }

    /// <summary>
    /// Use to list the community's participants.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <returns></returns>
    [HttpGet("{communityId}/Participiants")]
    public async Task<Response<List<UserDto>>> GetParticipiants(string communityId, int skip, int take)
    {
        return await _participiantService.GetParticipiants(this.Token, communityId, skip, take);
    }

    /// <summary>
    /// Search spesific user in the community participiants
    /// </summary>
    /// <param name="communityId"></param>
    /// <param name="q"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <returns></returns>
    [HttpGet("{communityId}/Participiants/Search")]
    public async Task<Response<List<UserDto>>> SearchParticipiant(string communityId, string q, int skip, int take)
    {
        return await _participiantService.SearchParticipiant(communityId, q, skip, take);
    }


    [HttpGet("join-requests")]
    public async Task<Response<List<UserDto>>> JoinRequests([FromForm] string communityId, int skip, int take)
    {
        return await _participiantService.GetJoinRequests(this.UserId, communityId, skip, take);
    }
       
    [HttpPost("{communityId}/accept-join-request")]
    public async Task<Response<NoContent>> AcceptJoinRequest(string communityId, string id)
    {
        return await _participiantService.AcceptUserJoinRequest(this.UserId, communityId, id);
    }
        
    [HttpPost("{communityId}/decline-join-request")]
    public async Task<Response<NoContent>> DeclineJoinRequest(string communityId, string id)
    {
        return await _participiantService.DeclineUserJoinRequest(this.UserId, communityId, id);
    }
}