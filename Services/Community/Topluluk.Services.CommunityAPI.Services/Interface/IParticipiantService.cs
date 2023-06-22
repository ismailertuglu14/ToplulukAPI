using Topluluk.Services.CommunityAPI.Model.Dto.Http;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Services.Interface;

public interface IParticipiantService
{
    Task<Response<string>> Join(string userId, string token, string communityId);
    Task<Response<NoContent>> Leave(string userId, string token, string communityId);
    
    Task<Response<NoContent>> AcceptUserJoinRequest(string userId, string communityId, string targetUserId);
    Task<Response<NoContent>> DeclineUserJoinRequest(string userId, string communityId, string targetUserId);
    Task<Response<List<UserDto>>> GetJoinRequests(string userId, string communityId, int skip = 0, int take = 10);
    
    Task<Response<List<UserDto>>> GetParticipiants(string token, string id, int skip = 0, int take = 10);
    Task<Response<List<UserDto>>> SearchParticipiant(string communityId, string name, int skip = 0, int take = 10);
    
}