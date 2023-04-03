using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RestSharp;
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
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.CommunityAPI.Services.Implementation
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly IMapper _mapper;
        private readonly ICapPublisher _capPublisher;
        private readonly RestClient _client;

        public CommunityService(ICommunityRepository communityRepository, IMapper mapper, ICapPublisher capPublisher)
        {
            _communityRepository = communityRepository;
            _mapper = mapper;
            _capPublisher = capPublisher;
            _client = new RestClient();
        }
        public async Task<Response<List<Community>>> GetCommunities()
        {
            DatabaseResponse communities = await _communityRepository.GetAllAsync(1,0,x=>x.IsVisible == true);
            return await Task.FromResult(Response<List<Community>>.Success(communities.Data, ResponseStatus.Success));
        }

        public async Task<Response<List<CommunityGetPreviewDto>>> GetCommunitySuggestions(string userId, HttpRequest request, int skip = 0, int take = 5)
        {
            try
            {
                DatabaseResponse response = await _communityRepository.GetAllAsync(take, skip, c => c.IsPublic != false && c.IsVisible != false && !c.Participiants.Contains(userId));
                List<CommunityGetPreviewDto> dto = _mapper.Map<List<CommunityGetPreviewDto>>(response.Data);


                return await Task.FromResult(Response<List<CommunityGetPreviewDto>>.Success(dto, ResponseStatus.Success));
            }
            catch(Exception e)
            {
                return await Task.FromResult(Response<List<CommunityGetPreviewDto>>.Fail($"Some error occured: {e}", ResponseStatus.InitialError));

            }
        }


        public async Task<Response<CommunityGetByIdDto>> GetCommunityById(string userId, string communityId)
        {
            Community? community = await _communityRepository.GetFirstCommunity(c => c.Id == communityId && c.IsVisible == true && c.IsRestricted == false);
            CommunityGetByIdDto _community = new();
            CommunityGetAdminDto adminDto = new();

            if (community == null )
            {
                return await Task.FromResult(Response<CommunityGetByIdDto>.Fail("Not found",ResponseStatus.NotFound));
            }

            if (community.IsDeleted == true)
            {
                return await Task.FromResult(Response<CommunityGetByIdDto>.Fail("Deleted", ResponseStatus.NotFound));
            }

            if (community.IsRestricted == true)
            {
                return await Task.FromResult(Response<CommunityGetByIdDto>.Fail("Restricted", ResponseStatus.NotFound));
            }

            if (community.IsVisible == false)
            {
                return await Task.FromResult(Response<CommunityGetByIdDto>.Fail("Not Visible public", ResponseStatus.NotFound));
            }

            // @@@@@@ starts here

            var request = new RestRequest("https://localhost:7202/user/communityOwner").AddQueryParameter("id", community.AdminId);
            var response = await _client.ExecuteGetAsync<Response<GetCommunityOwnerDto>>(request);
            _community.AdminId = response.Data.Data.OwnerId;
            _community.AdminName = response.Data.Data.Name;
            _community.AdminImage = response.Data.Data.ProfileImage;
            _community.Location = community.Location ?? "";
            _community.Title = community.Title;
            _community.Description = community.Description;
            _community.IsOwner = false;
            _community.CoverImage = community.CoverImage;
            _community.BannerImage = community.BannerImage;
            _community.ParticipiantsCount = community.Participiants.Count;
            _community.IsParticipiant = community.Participiants.Contains(userId);

            return await Task.FromResult(Response<CommunityGetByIdDto>.Success(_community, ResponseStatus.Success));
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

        public async Task<Response<string>> Delete(string ownerId,string communityId)
        {
            try
            {
                Community community = await _communityRepository.GetFirstAsync(c => c.Id == communityId);

                if (community.AdminId == ownerId)
                {
                    _communityRepository.DeleteById(communityId);
                    return await Task.FromResult(Response<string>.Success("Deleted", ResponseStatus.Success));
                }
                else
                {
                    return await Task.FromResult(Response<string>.Fail("Not authorized for delete community. You are not an admin!", ResponseStatus.NotAuthenticated));

                }


            }
            catch(Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured: {e}", ResponseStatus.NotAuthenticated));

            }

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

        public async Task<Response<string>> DeletePermanently(string ownerId, string communityId)
        {
            try
            {
                Community community = await _communityRepository.GetFirstAsync(c => c.Id == communityId);

                if (community.AdminId == ownerId)
                {
                    _communityRepository.DeleteCompletely(communityId);
                    return await Task.FromResult(Response<string>.Success("Deleted", ResponseStatus.Success));
                }
                else
                {
                    return await Task.FromResult(Response<string>.Fail("Not authorized for delete community. You are not an admin!", ResponseStatus.NotAuthenticated));

                }


            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured: {e}", ResponseStatus.InitialError));

            }
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

        public async Task<Response<List<CommunityGetPreviewDto>>> GetUserCommunities(string userId)
        {
            DatabaseResponse response = await _communityRepository.GetAllAsync(10, 0, c => c.Participiants.Contains(userId) && c.IsPublic == true && c.IsRestricted == false);
            List<CommunityGetPreviewDto> dto = _mapper.Map<List<CommunityGetPreviewDto>>(response.Data);
            return await Task.FromResult(Response<List<CommunityGetPreviewDto>>.Success(dto, ResponseStatus.Success));

        }

        public async Task<Response<bool>> CheckCommunityExist(string id)
        {
            Community community = await _communityRepository.GetFirstAsync(c => c.Id == id);
            if (community != null)
            {
                return await Task.FromResult(Response<bool>.Success(true, ResponseStatus.Success));
            }
            else
            {
                return await Task.FromResult(Response<bool>.Success(false, ResponseStatus.Success));
            }
        }

        public async Task<Response<CommunityInfoPostLinkDto>> GetCommunityInfoForPostLink(string id)
        {
            try
            {
                Community community = await _communityRepository.GetFirstAsync(c => c.Id == id);
                CommunityInfoPostLinkDto dto = _mapper.Map<CommunityInfoPostLinkDto>(community);
                return await Task.FromResult(Response<CommunityInfoPostLinkDto>.Success(dto, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<CommunityInfoPostLinkDto>.Fail($"Some error occured: {e}",
                    ResponseStatus.InitialError));
            }

        }

        public async Task<Response<bool>> CheckIsUserAdminOwner(string userId)
        {
            try
            {
                bool result = await _communityRepository.AnyAsync(c => c.AdminId == userId);
                return await Task.FromResult(Response<bool>.Success(result, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<bool>.Fail($"Some error occured: {e}",
                    ResponseStatus.InitialError));
            }
        }
    }
}

