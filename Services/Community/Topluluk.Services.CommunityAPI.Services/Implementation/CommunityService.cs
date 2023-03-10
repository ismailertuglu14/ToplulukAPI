﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Topluluk.Services.CommunityAPI.Data.Interface;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Dto.Http;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Services.CommunityAPI.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;
using Topluluk.Shared.Helper;
using static System.Net.Mime.MediaTypeNames;

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


        public async Task<Response<string>> GetCommunityById(string userId, string communityId)
        {
            Community? community = await _communityRepository.GetFirstCommunity(c => c.Id == communityId && c.IsVisible == true && c.IsRestricted == false);
            CommunityGetDto _community = new();
            CommunityGetAdminDto adminDto = new();

            if (community == null)
            {
                return await Task.FromResult(Response<string>.Fail("",ResponseStatus.NotFound));
            }

            if (community.AdminId == userId)
            {
                _community.IsOwner = true;
            }

            else if (community.ModeratorIds.Any(m => m.UserId == userId))
            {

            }

            return await Task.FromResult(Response<string>.Success(JsonConvert.SerializeObject(community), ResponseStatus.Success));
        }

        public async Task<Response<string>> Join(CommunityJoinDto communityInfo)
        {
            Community community = await _communityRepository.GetFirstAsync(c => c.Id == communityInfo.CommunityId );

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

            string slug = StringToSlugConvert(communityInfo.Title);
            bool isSluqUnique = false;
            byte index = 0;
            Community? existingCommunity = await _communityRepository.GetFirstAsync(c => c.Slug == slug);
            if (existingCommunity != null)
            {
                // If the slug already exists in the database, add a number to the end of the slug and save the new entity
                int number = 1;
                string newSlug = $"{slug}-{number}";
                while (await _communityRepository.GetFirstAsync(c=> c.Slug == newSlug) != null)
                {
                    number++;
                    newSlug = $"{slug}-{number}";
                }
                slug = newSlug;
            }

            Community community = _mapper.Map<Community>(communityInfo);
            community.Slug = slug;
            community.AdminId = communityInfo.CreatedById;
            community.Participiants.Add(communityInfo.CreatedById!);
            DatabaseResponse response = await _communityRepository.InsertAsync(community);

            if (communityInfo.CoverImage != null)
            {
                using var stream = new MemoryStream();
                await communityInfo.CoverImage.CopyToAsync(stream);
                var imageData = stream.ToArray();

                await _capPublisher.PublishAsync(QueueConstants.COMMUNITY_IMAGE_UPLOAD, new { CommunityId = response.Data, CoverImage = imageData , FileName = communityInfo.CoverImage.FileName });
            }

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

            Community community = await _communityRepository.GetFirstAsync(c => c.Id == dtoInfo.CommunityId);

            if(!community.Participiants.Contains(dtoInfo.UserId) || community.AdminId != dtoInfo.AdminId)
                return await Task.FromResult(Response<string>.Fail("Failed", ResponseStatus.NotAuthenticated));

            community.AdminId = dtoInfo.UserId;
            _communityRepository.Update(community);
            return await Task.FromResult(Response<string>.Success("Successfully updated new admin.", ResponseStatus.Success));
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

        public async Task<Response<string>> UpdateCoverImage(CommunityImageUploadedDto dto)
        {
            Community community = await _communityRepository.GetFirstAsync(c => c.Id == dto.CommunityId);
            community.CoverImage = dto.CoverImage;
            Console.WriteLine($"ismail DEBUG: {dto.CoverImage}");
            _communityRepository.Update(community);
            return await Task.FromResult(Response<string>.Success($"Success ${community.CoverImage}", ResponseStatus.Success));

        }

        private string StringToSlugConvert(string phrase)
        {
            char[] turkishChars = new char[] { 'ç', 'ğ', 'ı', 'i', 'ö', 'ş', 'ü' };
            char[] englishChars = new char[] { 'c', 'g', 'i', 'i', 'o', 's', 'u' };

            string str = phrase.ToLower();

            for (int i = 0; i < turkishChars.Length; i++)
            {
                str = str.Replace(turkishChars[i], englishChars[i]);
            }

            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space  
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it  
            str = Regex.Replace(str, @"\s", "-"); // hyphens  

            return str;
        }

        public Task<Response<string>> DeletePermanently(string ownerId, string communityId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<List<string>>> GetParticipiants(string id)
        {
            Community community = await _communityRepository.GetFirstAsync(c => c.Id == id);
            return await Task.FromResult(Response<List<string>>.Success(community.Participiants.ToList(), ResponseStatus.Success));
        }

        public async Task<Response<string>> PostCreated(PostCreatedCommunityDto dto)
        {
            var community = await _communityRepository.GetFirstAsync(c => c.Id == dto.CommunityId);
            // post Id

             //todocommunity.Posts.Add(dto.Id);
            _communityRepository.Update(community);
            return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));

        }

        public async Task<Response<string>> GetCommunityTitle(string id)
        {
            Community community = await _communityRepository.GetFirstAsync(c => c.Id == id);
            return await Task.FromResult(Response<string>.Success(community.Title, ResponseStatus.Success));
        }
    }
}

