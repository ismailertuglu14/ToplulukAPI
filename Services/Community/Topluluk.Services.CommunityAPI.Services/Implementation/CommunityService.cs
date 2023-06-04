
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using RestSharp;
using Topluluk.Services.CommunityAPI.Data.Interface;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Dto.Http;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Services.CommunityAPI.Services.Interface;
using Topluluk.Services.FileAPI.Model.Dto.Http;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Helper;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.CommunityAPI.Services.Implementation
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly IMapper _mapper;
        private readonly ICommunityParticipiantRepository _participiantRepository;
        private readonly RestClient _client;

        public CommunityService(ICommunityRepository communityRepository, IMapper mapper, ICommunityParticipiantRepository participiantRepository)
        {
            _communityRepository = communityRepository;
            _participiantRepository = participiantRepository;
            _mapper = mapper;
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
                DatabaseResponse response = await _communityRepository.GetAllAsync(take, skip, c => c.IsPublic != false
                                                                        && c.IsVisible != false );
                List<CommunityGetPreviewDto> dtos = _mapper.Map<List<CommunityGetPreviewDto>>(response.Data);
                var communityIds = dtos.Select(c => c.Id).ToList();
                var communityParticipiants = await _participiantRepository.GetCommunityParticipiants(communityIds);
                foreach (var dto in dtos)
                {
                    dto.ParticipiantsCount = communityParticipiants.FirstOrDefault(c => c.Key == dto.Id).Value;
                }
                return await Task.FromResult(Response<List<CommunityGetPreviewDto>>.Success(dtos, ResponseStatus.Success));
            }
            catch(Exception e)
            {
                return await Task.FromResult(Response<List<CommunityGetPreviewDto>>.Fail($"Some error occured: {e}", ResponseStatus.InitialError));

            }
        }
        public async Task<Response<List<CommunityInfoPostLinkDto>>> GetParticpiantsCommunities(string userId, string token)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<CommunityInfoPostLinkDto>>.Fail($"Some error occured: {e}", ResponseStatus.InitialError));

            }
        }


        public async Task<Response<int>> GetUserParticipiantCommunitiesCount(string userId)
        {
            try
            {
                int count = await _participiantRepository.Count(cp => !cp.IsDeleted && cp.UserId == userId);
                return Response<int>.Success(count, ResponseStatus.Success);
            }
            catch (Exception e)
            {
                return Response<int>.Fail($"Some error occurred: {e}", ResponseStatus.InitialError);
            }
        }

        public async Task<Response<CommunityGetByIdDto>> GetCommunityById(string userId, string token, string communityId)
        {
            Community? community = await _communityRepository.GetFirstCommunity(c => c.Id == communityId && c.IsVisible == true && c.IsRestricted == false);
            CommunityGetByIdDto _community = new();
            if (community == null || community.IsDeleted)
            {
                return await Task.FromResult(Response<CommunityGetByIdDto>.Fail("Not found",ResponseStatus.NotFound));
            }
            

            if (community.IsRestricted )
            {
                return await Task.FromResult(Response<CommunityGetByIdDto>.Fail("Restricted", ResponseStatus.NotFound));
            }

            if (community.IsVisible == false)
            {
                return await Task.FromResult(Response<CommunityGetByIdDto>.Fail("Not Visible public", ResponseStatus.NotFound));
            }
            
            var userRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/user/GetUserById").AddQueryParameter("userId", community.AdminId).AddHeader("Authorization",token);
            var userResponseTask =  _client.ExecuteGetAsync<Response<UserDto>>(userRequest);
            var participiantCountTask = _participiantRepository.Count(p => !p.IsDeleted && p.CommunityId == community.Id);
            var IsParticipiantTask =  _participiantRepository.AnyAsync(p => !p.IsDeleted && p.CommunityId == communityId && p.UserId == userId);
            
            await Task.WhenAll(userResponseTask, participiantCountTask, IsParticipiantTask);
            var user = userResponseTask.Result.Data.Data;
            _community.Id = communityId;
            _community.AdminId = user.Id;
            _community.AdminName = user.FirstName;
            _community.AdminLastName = user.LastName;
            _community.AdminImage = user.ProfileImage;
            _community.AdminGender = user.Gender;
            _community.Location = community.Location ?? "";
            _community.Title = community.Title;
            _community.Description = community.Description;
            _community.IsOwner = community.AdminId == userId ? true : false;
            _community.CoverImage = community.CoverImage;
            _community.BannerImage = community.BannerImage;
            _community.ParticipiantsCount = participiantCountTask.Result;
            _community.IsParticipiant = IsParticipiantTask.Result;
            
            return await Task.FromResult(Response<CommunityGetByIdDto>.Success(_community, ResponseStatus.Success));
        }

        public async Task<Response<string>> Join(string userId, string token, string communityId)
        {
            try
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

                if (!community.IsPublic)
                {
                    if (!community.JoinRequestWaitings.Contains(userId))
                    {
                        community.JoinRequestWaitings.Add(userId);
                        _communityRepository.Update(community);
                        return await Task.FromResult(Response<string>.Success("Send request", ResponseStatus.Success));
                    }
                    else
                    {
                        return await Task.FromResult(Response<string>.Success("You already send request this community", ResponseStatus.Success));
                    }
                }
                else
                {

                    CommunityParticipiant participiant = new()
                    {   
                        UserId = userId,
                        CommunityId = communityId,
                    };
                
                    await _participiantRepository.InsertAsync(participiant);

                    return await Task.FromResult(Response<string>.Success("Joined", ResponseStatus.Success));
                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured: {e}", ResponseStatus.NotAuthenticated));
            }            
        }

        public async Task<Response<string>> Create(string userId,string token, CommunityCreateDto communityInfo)
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
            community.AdminId = userId;

            if (communityInfo.CoverImage != null)
            {
                ///file/upload-community-cover
                byte[] imageBytes;

                using (var stream = new MemoryStream())
                {
                    communityInfo.CoverImage.CopyTo(stream);
                    imageBytes = stream.ToArray();
                }
                using (var client = new HttpClient())
                {
                    var content = new MultipartFormDataContent();
                    var imageContent = new ByteArrayContent(imageBytes);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg"); // Resim formatına uygun mediatype belirleme
                    content.Add(imageContent, "file", communityInfo.CoverImage.FileName); // "files": paramtere adı "files[0].FileName": Resimin adı
                    var imageResponse = await client.PostAsync(ServiceConstants.API_GATEWAY + "/file/upload-community-cover", content);

                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var responseData = await imageResponse.Content.ReadFromJsonAsync<Response<string>>();

                        if (responseData.Data != null)
                        {
                            community.CoverImage = responseData.Data;
                        }
                    }
                }

            }

            DatabaseResponse response = await _communityRepository.InsertAsync(community);
            CommunityParticipiant participiant = new CommunityParticipiant()
            {
                UserId = userId,
                CommunityId = response.Data,
            };
            await _participiantRepository.InsertAsync(participiant);
         
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
                    _participiantRepository.DeleteByExpression(p => p.CommunityId == communityId);
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


        public async Task<Response<NoContent>> KickUser(string token, string communityId, string userId)
        {
            try
            {
                string currentId = TokenHelper.GetUserIdByToken(token);

                Community? community = await _communityRepository.GetFirstAsync(c => c.Id == communityId && c.AdminId == currentId);
                
                if (community == null)
                    return Response<NoContent>.Fail("Community Not Found",ResponseStatus.NotFound);
                
                // Only admin can kick participiants.
                if(currentId != community.AdminId)
                    return Response<NoContent>.Fail("Unaturhoized for this feature.",ResponseStatus.Unauthorized);

                // Admin can't kick himself
                if(currentId == community.AdminId)
                    return Response<NoContent>.Fail("Admin can not kick yourself",ResponseStatus.BadRequest);

                CommunityParticipiant? participiant = await _participiantRepository.GetFirstAsync(p => p.CommunityId == communityId && p.UserId == userId);
                
                if(participiant == null)
                    return Response<NoContent>.Fail("User Not Found",ResponseStatus.NotFound);
                
                _participiantRepository.DeleteByExpression(p => p.CommunityId == communityId && p.UserId == userId);
                
                return Response<NoContent>.Success(ResponseStatus.Success);
            }
            catch (Exception e)
            {

                return Response<NoContent>.Fail(e.ToString(), ResponseStatus.InitialError);
            }
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

        public async Task<Response<string>> AssignUserAsAdmin(string userId, AssignUserAsAdminDto dtoInfo)
        {
            try
            {
                if (userId.IsNullOrEmpty() || dtoInfo.CommunityId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<string>.Fail("Bad Request", ResponseStatus.BadRequest));
                }

                Community community = await _communityRepository.GetFirstAsync(c => c.Id == dtoInfo.CommunityId);

                //   Admin yapılacak kişi participiant mı ?            Isteği atan kişi admin mi ?
                var isParticipiantTargetUser = await _participiantRepository.AnyAsync(p => p.CommunityId == community.Id && p.UserId == dtoInfo.UserId ); 
                if (!isParticipiantTargetUser || userId != community.AdminId)
                    return await Task.FromResult(Response<string>.Fail("Failed", ResponseStatus.NotAuthenticated));

                community.AdminId = dtoInfo.UserId;
                _communityRepository.Update(community);
                return await Task.FromResult(Response<string>.Success("Successfully updated new admin.", ResponseStatus.Success));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred : {e}", ResponseStatus.InitialError));
            }
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

        public async Task<Response<string>> UpdateCoverImage(string userId, string communityId, CoverImageUpdateDto dto)
        {
            Community? community = await _communityRepository.GetFirstAsync(c => c.Id == communityId && c.AdminId == userId);

            if (community == null)
            {
                return Response<string>.Fail("Community Not Found", ResponseStatus.NotFound);
            }

            if (dto.File == null)
                return Response<string>.Success(community.CoverImage ?? "", ResponseStatus.Success);
            
            byte[] imageBytes;

            using (var stream = new MemoryStream())
            {
                await dto.File.CopyToAsync(stream);
                imageBytes = stream.ToArray();
            }
            
            using (var client = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                Response<string>? responseData = new();

                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                content.Add(imageContent, "File", dto.File.FileName);


                if (!community.CoverImage.IsNullOrEmpty())
                {

                    NameObject nameObject = new() { Name = community.CoverImage! };
                    var deleteCoverImageRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/file/delete-community-cover-image").AddBody(nameObject);
                    var deleteCoverImageResponse = await _client.ExecutePostAsync<Response<string>>(deleteCoverImageRequest);
                }
                var response = await client.PostAsync("https://localhost:7165/file/upload-community-cover-image", content);
                if (response.IsSuccessStatusCode)
                {
                    responseData = await response.Content.ReadFromJsonAsync<Response<string>>(); 
                    if (responseData != null)
                    {
                        var imageUrl = responseData.Data;

                        community.CoverImage = imageUrl;
                        _communityRepository.Update(community);
                        return await Task.FromResult(Response<string>.Success(imageUrl, ResponseStatus.Success));
                    }

                    throw new Exception($"{typeof(CommunityService)} exception, IsSuccessStatusCode=true, responseData=null");
                }
                else
                {
                    // Resim yükleme işlemi başarısız
                    return await Task.FromResult(Response<string>.Fail("Failed while uploading image with http client", ResponseStatus.InitialError));
                }
                
            }

            
            
            
           throw new NotImplementedException();
        }

        public async Task<Response<NoContent>> UpdateBannerImage(string userId, string communityId, BannerImageUpdateDto dto)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                return Response<NoContent>.Fail(e.ToString(), ResponseStatus.InitialError);
            }
        }

        public async Task<Response<List<CommunityGetPreviewDto>>> ParticipiantCommunities(string sourceId, string targetId)
        {
            try
            {
                var participiants =  _participiantRepository.GetListByExpressionPaginated(0, 10, c => c.UserId == targetId && (sourceId == targetId || c.IsShownOnProfile));
                List<string> idList = participiants.Select(p => p.CommunityId).ToList(); 
                var communities = _communityRepository.GetListByExpression(c => idList.Contains(c.Id));
                List<CommunityGetPreviewDto> dtos = _mapper.Map<List<CommunityGetPreviewDto>>(communities);
                foreach (var dto in dtos)
                {
                    dto.ParticipiantsCount =
                        await _participiantRepository.Count(p => !p.IsDeleted && p.CommunityId == dto.Id);
                }
                return await Task.FromResult(Response<List<CommunityGetPreviewDto>>.Success(dtos, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return Response<List<CommunityGetPreviewDto>>.Fail(e.ToString(),ResponseStatus.InitialError);
            }
        }


        private string StringToSlugConvert(string phrase)
        {
            var turkishChars = new char[] { 'ç', 'ğ', 'ı', 'i', 'ö', 'ş', 'ü' };
            var englishChars = new char[] { 'c', 'g', 'i', 'i', 'o', 's', 'u' };

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

        public async Task<Response<List<UserDto>>> GetParticipiants(string token, string id)
        {
            Community? community = await _communityRepository.GetFirstAsync(c => c.Id == id);
            
            var participiants = _participiantRepository.GetListByExpression(c => c.CommunityId == id && c.UserId != community.AdminId);
            var idList = new IdList() { ids = participiants.Select(p => p.UserId).ToList() };
            var usersRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/user/get-user-info-list")
                                    .AddHeader("Authorization",token)                 
                                    .AddBody(idList);
            
            var usersResponse = await _client.ExecutePostAsync<Response<List<UserDto>>>(usersRequest);
    
            if (!usersResponse.IsSuccessful)
                return Response<List<UserDto>>.Fail("Failed", ResponseStatus.Failed);
            
            return Response<List<UserDto>>.Success(usersResponse.Data.Data, ResponseStatus.Success);
        }


        public async Task<Response<string>> GetCommunityTitle(string id)
        {
            Community community = await _communityRepository.GetFirstAsync(c => c.Id == id);
            return await Task.FromResult(Response<string>.Success(community.Title, ResponseStatus.Success));
        }

        public async Task<Response<List<CommunityGetPreviewDto>>> GetUserCommunities(string userId)
        {
            var participiants =  _participiantRepository.GetListByExpressionPaginated(0, 10, c => c.UserId == userId);
            List<string> idList = participiants.Select(p => p.CommunityId).ToList(); 
            var communities = _communityRepository.GetListByExpression(c => idList.Contains(c.Id));
            List<CommunityGetPreviewDto> dto = _mapper.Map<List<CommunityGetPreviewDto>>(communities);
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
                bool result = await _communityRepository.AnyAsync(c => !c.IsDeleted && c.AdminId == userId);
                return await Task.FromResult(Response<bool>.Success(result, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<bool>.Fail($"Some error occured: {e}",
                    ResponseStatus.InitialError));
            }
        }

        public async Task<Response<bool>> LeaveUserDelete(string id, IdList list)
        {
            try
            {
                if (id.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<bool>.Fail("Bad Request", ResponseStatus.BadRequest));
                }

                return await Task.FromResult(Response<bool>.Success(true, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<bool>.Fail($"Some error occured: {e}",
                    ResponseStatus.InitialError));
            }
        }
    }
}

