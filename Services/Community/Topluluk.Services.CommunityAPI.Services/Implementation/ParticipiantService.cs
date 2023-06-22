using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using Topluluk.Services.CommunityAPI.Data.Interface;
using Topluluk.Services.CommunityAPI.Model.Dto.Http;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Services.CommunityAPI.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Exceptions;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.CommunityAPI.Services.Implementation;

public class ParticipiantService : IParticipiantService
{

    private readonly ICommunityRepository _communityRepository;
    private readonly ICommunityParticipiantRepository _participiantRepository;
    private readonly IMapper _mapper;
    private readonly RestClient _client;
    public ParticipiantService(ICommunityParticipiantRepository participiantRepository, ICommunityRepository communityRepository, IMapper mapper)
    {
        _participiantRepository = participiantRepository;
        _communityRepository = communityRepository;
        _mapper = mapper;
        _client = new RestClient();
    }
    
    public async Task<Response<string>> Join(string userId, string token, string communityId)
    {
            
               Community community = await _communityRepository.GetFirstAsync(c => c.Id == communityId ); 
               var participiants =
                    _participiantRepository.GetListByExpression(p => p.UserId == userId && p.CommunityId == community.Id);
                
                if (participiants.Any(p => p.UserId == userId))
                {
                    return await Task.FromResult(Response<string>.Success("You have already participiants this community", ResponseStatus.Success));
                }

                if (community.IsRestricted && !community.IsVisible && community.BlockedUsers.Contains(userId) )
                {
                    return await Task.FromResult(Response<string>.Fail("Not found community", ResponseStatus.NotFound));
                }

                if ( participiants.Count  >= community.ParticipiantLimit)
                {
                    return await Task.FromResult(Response<string>.Fail("Community full now!", ResponseStatus.Failed));
                }

                CommunityParticipiant participiant = new()
                {   
                    UserId = userId,
                    CommunityId = communityId,
                };
                
                if (!community.IsPublic)
                {
                    var isRequested = await _participiantRepository.AnyAsync(c => !c.IsDeleted 
                                                                        && c.CommunityId == communityId && c.UserId == userId 
                                                                        && c.Status != ParticipiantStatus.ACCEPTED);
                    if (!isRequested)
                    {
                        participiant.Status = ParticipiantStatus.REQUESTED;
                        await _participiantRepository.InsertAsync(participiant);
                        return await Task.FromResult(Response<string>.Success("Send request", ResponseStatus.Success));
                    }

                    return await Task.FromResult(Response<string>.Success("You already send request this community", ResponseStatus.Success));
                }


                participiant.Status = ParticipiantStatus.ACCEPTED;
                await _participiantRepository.InsertAsync(participiant);

                return await Task.FromResult(Response<string>.Success("Joined", ResponseStatus.Success));
          
    }
    public async Task<Response<NoContent>> Leave(string userId, string token, string communityId)
    {
        try
        {
            if (userId.IsNullOrEmpty())
            {
                return await Task.FromResult(Response<NoContent>.Fail("", ResponseStatus.BadRequest));
            }

            Community? community = await _communityRepository.GetFirstAsync(c => c.Id == communityId);

            if (community == null)
            {
                return await Task.FromResult(Response<NoContent>.Fail("Community Not Found", ResponseStatus.NotFound));
            }
            if (community.AdminId == userId)
            {
                return await Task.FromResult(Response<NoContent>.Fail("You are the owner of this community!", ResponseStatus.CommunityOwnerExist));
            }

            CommunityParticipiant? participiant= await _participiantRepository.GetFirstAsync(p => p.UserId == userId && p.CommunityId == communityId);
            if (participiant != null)
            {
                _participiantRepository.DeleteCompletely(participiant.Id);
                return await Task.FromResult(Response<NoContent>.Success( ResponseStatus.Success));
            }

            return await Task.FromResult(
                Response<NoContent>.Fail("You are not participiant", ResponseStatus.Failed));
        }
        catch (Exception e)
        {
            return await Task.FromResult(Response<NoContent>.Fail($"Some error occurred : {e}", ResponseStatus.InitialError));
        }
    }
    public async Task<Response<NoContent>> AcceptUserJoinRequest(string userId, string communityId, string targetUserId)
    {
        Community community = await _communityRepository.GetFirstAsync(c => c.Id == communityId);

        if (community.AdminId != userId)
            throw new UnauthorizedAccessException();

        CommunityParticipiant participiantRequest = await _participiantRepository.GetFirstAsync(p =>
            p.CommunityId == communityId && p.UserId == targetUserId && p.Status == ParticipiantStatus.REQUESTED);
            
        if (participiantRequest == null)
            throw new ArgumentNullException();

        participiantRequest.Status = ParticipiantStatus.ACCEPTED;
        _participiantRepository.Update(participiantRequest);
        return Response<NoContent>.Success(ResponseStatus.Success);
    }
    public async Task<Response<NoContent>> DeclineUserJoinRequest(string userId, string communityId, string targetUserId)
    {
        Community community = await _communityRepository.GetFirstAsync(c => c.Id == communityId);

        if (community.AdminId != userId)
            throw new UnauthorizedAccessException();

        CommunityParticipiant participiantRequest = await _participiantRepository.GetFirstAsync(p =>
            p.CommunityId == communityId && p.UserId == targetUserId && p.Status == ParticipiantStatus.REQUESTED);
            
        if (participiantRequest == null)
            throw new ArgumentNullException();

        participiantRequest.Status = ParticipiantStatus.REJECTED;
        _participiantRepository.Update(participiantRequest);
        return Response<NoContent>.Success(ResponseStatus.Success);
    }
    public async Task<Response<List<UserDto>>> GetJoinRequests(string userId, string communityId, int skip = 0, int take = 10)
    {
        Community? community = await _communityRepository.GetFirstAsync(c =>c.Id == communityId);
        
        if (community == null)
            throw new NotFoundException();

        if (community.AdminId != userId)
            throw new UnauthorizedAccessException();

        var joinRequests =_participiantRepository.GetListByExpressionPaginated(skip, take, 
            p => p.CommunityId == communityId && p.Status == ParticipiantStatus.REQUESTED);

        IdList userIds = new()
        {
            ids = joinRequests.Select(p => p.UserId).ToList()
        };

        var usersRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/user/get-user-info-list").AddBody(userIds);
        var usersResponse = await _client.ExecutePostAsync<Response<List<UserDto>>>(usersRequest);

        if (!usersResponse.IsSuccessful || usersResponse.Data == null )
            throw new Exception();

     
        return Response<List<UserDto>>.Success(usersResponse.Data.Data, ResponseStatus.Success);
    }
    public async Task<Response<List<UserDto>>> GetParticipiants(string token, string id, int skip = 0, int take = 10)
        {
            Community? community = await _communityRepository.GetFirstAsync(c => c.Id == id);
            
            if(community == null)
                return Response<List<UserDto>>.Success(null, ResponseStatus.Success);

            
            var participiants = _participiantRepository.GetListByExpressionPaginated(skip, take, c => c.CommunityId == id && c.UserId != community.AdminId && c.Status == ParticipiantStatus.ACCEPTED);
            var idList = new IdList() { ids = participiants.Select(p => p.UserId).ToList() };
            var usersRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/user/get-user-info-list")
                                    .AddHeader("Authorization",token)                 
                                    .AddBody(idList);
            
            var usersResponse = await _client.ExecutePostAsync<Response<List<UserDto>>>(usersRequest);
    
            if (!usersResponse.IsSuccessful)
                return Response<List<UserDto>>.Fail("Failed", ResponseStatus.Failed);
            
            return Response<List<UserDto>>.Success(usersResponse.Data!.Data, ResponseStatus.Success);
        }
        
        public async Task<Response<List<UserDto>>> SearchParticipiant(string communityId, string query, int skip = 0, int take = 10)
        {
            List<CommunityParticipiant> participiants = await _participiantRepository.GetListByExpressionAsync(p => p.CommunityId == communityId);
            IdList participiantIds = new() { ids = participiants.Select(p => p.UserId).ToList() };

            var usersRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/user/get-user-info-list").AddBody(participiantIds);
            var usersResponse = await _client.ExecutePostAsync<Response<List<UserDtoWithUserName>>>(usersRequest);
            
            if (!usersResponse.IsSuccessful)
                throw new Exception();
            
            if (usersResponse.Data?.Data == null || usersResponse.Data.Data.Count == 0)
            {
                return Response<List<UserDto>>.Success(new List<UserDto>(),ResponseStatus.Success);
            }
            var filteredUsers = usersResponse.Data.Data.Where(u => 
                                     (u.FirstName.ToLower() + u.LastName.ToLower()).Replace(" ","").Contains(query.Replace(" ","").ToLower(), StringComparison.OrdinalIgnoreCase)
                                     || u.UserName.ToLower().Contains(query.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
            
            List<UserDto> dtos = _mapper.Map<List<UserDto>>(filteredUsers);
            return Response<List<UserDto>>.Success(dtos,ResponseStatus.Success);
        }

}