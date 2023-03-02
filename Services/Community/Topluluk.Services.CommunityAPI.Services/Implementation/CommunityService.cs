using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;
using Topluluk.Services.CommunityAPI.Data.Interface;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Services.CommunityAPI.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;
using Topluluk.Shared.Helper;

namespace Topluluk.Services.CommunityAPI.Services.Implementation
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly IMapper _mapper;
        private readonly ICapPublisher _capPublisher;
        public CommunityService(ICommunityRepository communityRepository, IMapper mapper, ICapPublisher capPublisher)
        {
            _communityRepository = communityRepository;
            _mapper = mapper;
            _capPublisher = capPublisher;
        }
        public async Task<Response<List<Community>>> GetCommunities()
        {
            DatabaseResponse communities = await _communityRepository.GetAllAsync(1,0,x=>x.IsVisible == true);
            return await Task.FromResult(Response<List<Community>>.Success(communities.Data, ResponseStatus.Success));
        }

        public async Task<Response<List<object>>> GetCommunitySuggestions(int skip, int take,HttpRequest request)
        {
            
            DatabaseResponse response = await _communityRepository.GetAllAsync(take,skip,c => c.IsPublic == true);

            if (request.Headers["User-Agent"].ToString().Contains("Mobile"))
            {
                List<CommunitySuggestionMobileDto> suggestions = _mapper.Map<List<CommunitySuggestionMobileDto>>(response.Data);

                return await Task.FromResult(Response<List<object>>.Success(new(suggestions), ResponseStatus.Success));
            }
            else if(request.Headers["User-Agent"].ToString().Contains("Web"))
            {
                List<CommunitySuggestionWebDto> suggestions = _mapper.Map<List<CommunitySuggestionWebDto>>(response.Data);
                return await Task.FromResult(Response<List<object>>.Success(new(suggestions), ResponseStatus.Success));
            }
            return await Task.FromResult(Response<List<object>>.Fail("User-Agent cant null", ResponseStatus.Success));
        }

        public async Task<Response<string>> Join(CommunityJoinDto communityInfo)
        {
            Community community = await _communityRepository.GetFirstAsync(c => c.Id == communityInfo.CommunityId);

            if (community.Participiants.Contains(communityInfo.UserId))
            {
                return await Task.FromResult(Response<string>.Success("You have already participiants this community", ResponseStatus.Success));
            }

            if (community.IsRestricted && !community.IsVisible && community.BlockedUsers.Contains(communityInfo.UserId) )
            {
                return await Task.FromResult(Response<string>.Fail("Not found community", ResponseStatus.NotFound));
            }

            if (!community.IsPublic)
            {
                if (!community.JoinRequestWaitings.Contains(communityInfo.UserId))
                {
                    community.JoinRequestWaitings.Add(communityInfo.UserId);
                    _communityRepository.Update(community);
                    await _capPublisher.PublishAsync<CommunityUserJoinDto>("community.user.communityjoinrequest",
                        new() { UserId = communityInfo.UserId, CommunityId = communityInfo.CommunityId });
                    return await Task.FromResult(Response<string>.Success("Send request", ResponseStatus.Success));
                }
                else
                {
                    return await Task.FromResult(Response<string>.Success("You already send request this community", ResponseStatus.Success));
                }
            }
            else if (community.IsPublic)
            {
                community.Participiants.Add(communityInfo.UserId);
                _communityRepository.Update(community);

                await _capPublisher.PublishAsync<CommunityUserJoinDto>("community.user.communityjoin",
                    new() { UserId = communityInfo.UserId, CommunityId = communityInfo.CommunityId });
                return await Task.FromResult(Response<string>.Success("Joined", ResponseStatus.Success));
            }

            throw new Exception($"{nameof(CommunityService)} exception: Katılma issteği esnasında bir hata meydana geldi");
        }

        public async Task<Response<string>> Create(CommunityCreateDto communityInfo)
        {
            Community community = _mapper.Map<Community>(communityInfo);
            community.AdminId = communityInfo.CreatedById;
            community.Participiants.Add(communityInfo.CreatedById!);
            DatabaseResponse response = await _communityRepository.InsertAsync(community);
            await _capPublisher.PublishAsync<CommunityUserJoinDto>(QueueConstants.COMMUNITY_CREATE_USER_UPDATE, new() { UserId = communityInfo.CreatedById, CommunityId = response.Data });
            //var httpResponse = await HttpRequestHelper.handle(new{ UserId = communityInfo.CreatedById, CommunityId = response.Data }, "https://localhost:7202/user/updatecommunities", HttpType.POST);
            return await Task.FromResult(Response<string>.Success(response.Data, ResponseStatus.Success));
        }

        public Task<Response<string>> AcceptUserJoinRequest()
        {
            throw new NotImplementedException();
        }


        public Task<Response<string>> DeclineUserJoinRequest()
        {
            throw new NotImplementedException();
        }

        public async Task<Response<string>> Delete(string communityId)
        {
            DatabaseResponse response = _communityRepository.DeleteById(communityId);
            if(response.IsSuccess == true)
                return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));

            return await Task.FromResult(Response<string>.Fail("Failed while deleting community", ResponseStatus.Success));

        }


        public Task<Response<string>> KickUser()
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> Leave()
        {
            throw new NotImplementedException();
        }

        public async Task<Response<string>> AssignUserAsAdmin(AssignUserAsAdminDto dtoInfo)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<string>> AssignUserAsModerator(AssignUserAsModeratorDto dtoInfo)
        {
            Community community = await _communityRepository.GetFirstAsync(c => c.Id == dtoInfo.CommunityId);

            // Is it a moderator or admin who will assign the user as a moderator?
            if (community.ModeratorIds.FirstOrDefault(m => m.UserId == dtoInfo.UserId) == null && community.AdminId == dtoInfo.AssignedById || community.ModeratorIds.FirstOrDefault(m => m.UserId == dtoInfo.AssignedById) != null )
            {
                community.ModeratorIds.Add(new() { UserId = dtoInfo.UserId, AssignedById = dtoInfo.AssignedById });
                var response = _communityRepository.Update(community);
                return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));
            }
            else
            {
                return await Task.FromResult(Response<string>.Fail("Failed", ResponseStatus.InitialError));

            }
        }

     
    }
}

