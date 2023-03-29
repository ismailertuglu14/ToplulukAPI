using System;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using Topluluk.Services.User.Data.Interface;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Services.User.Model.Dto.Http;
using Topluluk.Services.User.Model.Entity;
using Topluluk.Services.User.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;
using Topluluk.Shared.Helper;
using ZstdSharp.Unsafe;
using _User = Topluluk.Services.User.Model.Entity.User;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.User.Services.Implementation
{
	public class UserService : IUserService
	{
        private readonly IUserRepository _userRepository;
        private readonly ICapPublisher _capPublisher;
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        public UserService(IUserRepository userRepository,ICapPublisher capPublisher, IMapper mapper)
		{
            _userRepository = userRepository;
            _capPublisher = capPublisher;
            _mapper = mapper;
            _client = new RestClient();
		}

        public async Task<Response<GetUserByIdDto>> GetUserById(string id,string userId)
        {
            _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
            
            if(user != null)
            {
                GetUserByIdDto dto = new();
                dto.Id = user.Id;
                dto.FirstName = user.FirstName;
                dto.LastName = user.LastName;
                dto.UserName = user.UserName;
                dto.ProfileImage = user.ProfileImage;
                dto.BannerImage = user.BannerImage;
                dto.IsPrivate = user.IsPrivate;
                dto.IsFollowing = user.Followers!.Contains(id);
                dto.Gender = user.Gender ?? GenderEnum.Unspecified;
                dto.FollowersCount = user.Followers.Count();
                dto.FollowingCount = user.Followings!.Count();


                return await Task.FromResult(Response<GetUserByIdDto>.Success(dto, ResponseStatus.Success));
            }
            return await Task.FromResult(Response<GetUserByIdDto>.Fail("", ResponseStatus.NotFound));

        }

        // Use this function instead of GetUserById
        public async Task<Response<GetUserByIdDto>> GetUserByUserName(string userName)
        {
            _User? user = await _userRepository.GetFirstAsync(u => u.UserName == userName);

            if (user != null)
            {
                GetUserByIdDto dto = new();
                dto.Id = user.Id;
                dto.FirstName = user.FirstName;
                dto.LastName = user.LastName;
                dto.UserName = user.UserName;
                dto.ProfileImage = user.ProfileImage;
                dto.BannerImage = user.BannerImage;
                dto.IsPrivate = user.IsPrivate;
                dto.FollowersCount = user.Followers.Count();
                dto.FollowingCount = user.Followings.Count();

             


                return await Task.FromResult(Response<GetUserByIdDto>.Success(dto, ResponseStatus.Success));
            }

            return await Task.FromResult(Response<GetUserByIdDto>.Fail("User not found!", ResponseStatus.NotFound));
        }

        public async Task<Response<string>> InsertUser(UserInsertDto userInfo)
        {
            _User user = _mapper.Map<_User>(userInfo);
            DatabaseResponse response = await _userRepository.InsertAsync(user);

            return await Task.FromResult(Response<string>.Success(response.Data,ResponseStatus.Success));
        }

        public async Task<Response<string>> DeleteUserById(string id, string token, UserDeleteDto dto)
        {
            
            try
            {
                if (id == dto.userId)
                {
                    // Topluluğu var mı kontrol et, varsa fail dön

                    // Post, PostComment, Event, EventComment leri deleted yap.

                    // Kullanıcıyı sil.
                    DatabaseResponse response = _userRepository.DeleteById(id);
                    // http request to user credential

                    var userDeleteRequest = new RestRequest("https://localhost:7232/authentication/delete").AddBody(dto).AddHeader("Authorization",token);
                    var userDeleteResponse = await _client.ExecutePostAsync<Response<string>>(userDeleteRequest);
                    if (userDeleteResponse.Data.IsSuccess == true)
                    {
                        return await Task.FromResult(Response<string>.Success("Successfully Deleted", ResponseStatus.Success));

                    }
                    else
                    {
                        return await Task.FromResult(Response<string>.Fail("UnAuthorized", ResponseStatus.NotAuthenticated));

                    }

                }
                else
                {
                    return await Task.FromResult(Response<string>.Fail("UnAuthorized", ResponseStatus.NotAuthenticated));
                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> FollowUser(UserFollowDto userFollowInfo)
        {

            _User sourceUser = await _userRepository.GetFirstAsync(u => u.Id == userFollowInfo.SourceId);
            _User targetUser = await _userRepository.GetFirstAsync(u => u.Id == userFollowInfo.TargetId);

            if (targetUser.Followers!.Contains(sourceUser.Id))
            {
                return await Task.FromResult(Response<string>.Success("Already following", ResponseStatus.Success));
            }

            if (targetUser.IsPrivate == true)
            {
                bool isContainSourceId = targetUser.IncomingFollowRequests!.Contains(sourceUser.Id);

                if (isContainSourceId != true)
                {
                    targetUser.IncomingFollowRequests!.Add(sourceUser.Id);
                    sourceUser.OutgoingFollowRequests!.Add(targetUser.Id);
                    List<_User> usersUpdate = new() { sourceUser, targetUser };
                    _userRepository.BulkUpdate(usersUpdate);
                }

                return await Task.FromResult(Response<string>.Success("Successfully follow request sent!", ResponseStatus.Success));
            }
            else
            {
                bool isContainSourceId = targetUser.Followers!.Contains(sourceUser.Id);

                if (isContainSourceId != true)
                {
                    targetUser.Followers!.Add(sourceUser.Id);
                    sourceUser.Followings!.Add(targetUser.Id);
                    List<_User> usersUpdate = new() { sourceUser, targetUser };
                    _userRepository.BulkUpdate(usersUpdate);
                }

                return await Task.FromResult(Response<string>.Success("Successfully followed!", ResponseStatus.Success));
            }

        }

        public async Task<Response<string>> UnFollowUser(UserFollowDto userFollowInfo)
        {
            _User sourceUser = await _userRepository.GetFirstAsync(u => u.Id == userFollowInfo.SourceId);
            _User targetUser = await _userRepository.GetFirstAsync(u => u.Id == userFollowInfo.TargetId);

            bool isSourceFollowingTarget = targetUser.Followers!.Contains(sourceUser.Id);

            if (isSourceFollowingTarget)
            {
                targetUser.Followers.Remove(sourceUser.Id);
                sourceUser.Followings!.Remove(targetUser.Id);

                List<_User> usersUpdate = new() { sourceUser, targetUser };
                _userRepository.BulkUpdate(usersUpdate);
            }

            return await Task.FromResult(Response<string>.Success("Successfully unfollowed!", ResponseStatus.Success));
        }

        public async Task<Response<string>> RemoveUserFromFollowers(UserFollowDto userInfo)
        {
            try
            {
                _User sourceUser = await _userRepository.GetFirstAsync(u => u.Id == userInfo.SourceId);
                _User targetUser = await _userRepository.GetFirstAsync(u => u.Id == userInfo.TargetId);

                bool isTargetFollowingSource = sourceUser.Followers!.Contains(targetUser.Id);

                if (isTargetFollowingSource)
                {
                    sourceUser.Followers.Remove(targetUser.Id);
                    targetUser.Followings!.Remove(sourceUser.Id);

                    List<_User> usersUpdate = new() { sourceUser, targetUser };
                    _userRepository.BulkUpdate(usersUpdate);
                }

                return await Task.FromResult(Response<string>.Success("Successfully unfollowed!", ResponseStatus.Success));
            }
            catch
            {
                return await Task.FromResult(Response<string>.Fail("Some error occured", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> BlockUser(string sourceId, string targetId)
        {
            // Kullanıcı blocklanırken source ve target userlardaki takip ve takipçi listesinden
            // birbirlerini temizlemeliyiz.
            // Sadece sourceUser' ın blockedAccount listesine targetUser' ın Id bilgisi verilmeli.
            _User sourceUser = await _userRepository.GetFirstAsync(u => u.Id == sourceId);
            _User targetUser = await _userRepository.GetFirstAsync(u => u.Id == targetId);
            if (sourceUser.BlockedUsers == null)
                sourceUser.BlockedUsers = new List<string>();

            if (sourceUser.BlockedUsers!.Contains(targetId))
                return await Task.FromResult(Response<string>.Success("This user already blocked.", ResponseStatus.Success));

            sourceUser.BlockedUsers!.Add(targetId);

            if (sourceUser.Followings!.Contains(targetId))
            {
                sourceUser.Followings.Remove(targetId);
                targetUser.Followers!.Remove(sourceId);
            }

            if (sourceUser.Followers!.Contains(targetId))
            {
                sourceUser.Followers.Remove(targetId);
                targetUser.Followings!.Remove(sourceId);
            }

            // todo Update both users
            _userRepository.BulkUpdate(new() { sourceUser,targetUser });
            return await Task.FromResult(Response<string>.Success("User blocked successfully.", ResponseStatus.Success));
        }

        public async Task<Response<List<UserSuggestionsDto>>> GetUserSuggestions(string userId, int limit = 5)
        {
            /* id, image, firstName, lastName, userName, isPrivate(direkt follow edip edememe durumu için gerekli.)
             */

            _User currentUser = await _userRepository.GetFirstAsync(u => u.Id == userId);

            // Sorgulanan kullanıcı bizi bloklamamış olmalı
            // Bizim sorguladığımız kullanıcıyı bloklamamış olmamız lazım.
            DatabaseResponse response = await _userRepository.GetAllAsync(limit,0, u => u.IsDeleted == false);
            //u.BlockedUsers!.Contains("") == false
            //&& currentUser.BlockedUsers!.Contains(u.Id) == false
            List<UserSuggestionsDto> userSuggestions = _mapper.Map<List<UserSuggestionsDto>>(response.Data);

            return await Task.FromResult(Response<List<UserSuggestionsDto>>.Success(userSuggestions, ResponseStatus.Success));

        }

        public Task<Response<List<UserSuggestionsDto>>> GetUserSuggestionsMore(int skip = 0, int take = 5)
        {
            throw new NotImplementedException();
        }

        // Mesaj ekranındaki search 
        public async Task<Response<List<UserSearchResponseDto>>?> SearchUser(string? text, string userId, int skip = 0, int take = 10)
        {
            if (text == null)
                return await Task.FromResult(Response<List<UserSearchResponseDto>>.Success(null, ResponseStatus.Success));

            DatabaseResponse response = await _userRepository.GetAllAsync(take,skip, u => u.Id != userId && u.UserName.Contains(text) || u.FirstName.Contains(text) || u.LastName.Contains(text) );



            List<UserSearchResponseDto> users = _mapper.Map<List<_User>, List<UserSearchResponseDto>>(response.Data);
            
            return await Task.FromResult(Response<List<UserSearchResponseDto>>.Success(users, ResponseStatus.Success));
        }

        public async Task<Response<string>> ChangeProfileImage(string userName, IFormFileCollection files)
        {
            _User user = await _userRepository.GetFirstAsync(u => u.UserName == userName);
            Response<List<string>>? responseData = new();
            byte[] imageBytes;

            using (var stream = new MemoryStream())
            {
                files[0].CopyTo(stream);
                imageBytes = stream.ToArray();
            }

            using (var client = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg"); // Resim formatına uygun mediatype belirleme
                content.Add(imageContent, "files", files[0].FileName); // "files": paramtere adı "files[0].FileName": Resimin adı

                if (user.ProfileImage != null)
                {

                    HttpResponseMessage responseMessage = await HttpRequestHelper.handle(user.ProfileImage, "https://localhost:7165/file/deleteuserimage", HttpType.POST);

                }

                var response = await client.PostAsync("https://localhost:7165/file/uploaduserimage", content);
                
                if (response.IsSuccessStatusCode)
                {
                    responseData = await response.Content.ReadFromJsonAsync<Response<List<string>>>();
                    if (responseData != null)
                    {
                        var imageUrl = responseData.Data[0];

                        user.ProfileImage = imageUrl;
                        _userRepository.Update(user);
                        return await Task.FromResult(Response<string>.Success($"Image changed with {imageUrl}", ResponseStatus.Success));
                    }

                     throw new Exception($"{typeof(UserService)} exception, IsSuccessStatusCode=true, responseData=null");
                }
                else
                {
                    // Resim yükleme işlemi başarısız
                    return await Task.FromResult(Response<string>.Fail("Failed while uploading image with http client", ResponseStatus.InitialError));
                }
            }
        }

        public async Task<Response<string>> ChangeBannerImage(UserChangeBannerDto changeBannerDto)
        {
            
            _User user = await _userRepository.GetFirstAsync(u => u.Id == changeBannerDto.UserId);
            using var stream = new MemoryStream();
            await changeBannerDto.File.CopyToAsync(stream);
            var imageData = stream.ToArray();

            if (user.BannerImage != null)
            {
                await _capPublisher.PublishAsync<UserChangeBannerDto>(QueueConstants.USER_DELETE_BANNER, new(){ UserId = changeBannerDto.UserId, File = changeBannerDto.File });
            }

            await _capPublisher.PublishAsync(QueueConstants.USER_CHANGE_BANNER, new { UserId = changeBannerDto.UserId, FileName = changeBannerDto.File.FileName, BannerImage = imageData });

            return await Task.FromResult(Response<string>.Success("", ResponseStatus.Success));
        }

        // todo FollowingRequest leri kullanıcı adı, ad, soyad, id, kullanıcı resmi ve istek attığı tarih ile dön.
        public async Task<Response<GetUserAfterLoginDto>> GetUserAfterLogin(string id)
        {
            GetUserAfterLoginDto dto = new();

            try
            {
                _User user = await _userRepository.GetFirstAsync(u => u.Id == id);
                if (user == null) throw new Exception("User not found");

                dto = _mapper.Map<GetUserAfterLoginDto>(user);


                return await Task.FromResult(Response<GetUserAfterLoginDto>.Success(dto, ResponseStatus.Success));

            }
            catch
            {
                return await Task.FromResult(Response<GetUserAfterLoginDto>.Fail("Failed", ResponseStatus.NotAuthenticated));

            }
        }

        public async Task<Response<string>> PrivacyChange(string userId, UserPrivacyChangeDto dto)
        {
            try
            {
                if (userId == dto.UserId)
                {
                    _User user = await _userRepository.GetFirstAsync(u => u.Id == dto.UserId);

                    if (user != null)
                    {
                        user.IsPrivate = dto.IsPrivate;
                        _userRepository.Update(user);
                        return await Task.FromResult(Response<string>.Success($"Privacy status Successfully updated to {user.IsPrivate}", ResponseStatus.Success));
                    }

                    return await Task.FromResult(Response<string>.Fail("User Not Found", ResponseStatus.NotAuthenticated));

                }
                else
                {
                    return await Task.FromResult(Response<string>.Fail("UnAuthorized", ResponseStatus.NotAuthenticated));

                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", ResponseStatus.InitialError));

            }
        }

        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ For Http calls coming from other services @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\



        public async Task<Response<string>> UpdateCommunities(string userId, string communityId)
        {
            _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
            user.Communities?.Add(communityId);
            _userRepository.Update(user);
            return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));
        }

        public async Task UserBanngerChanged(string userId, string fileName)
        {
            _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);

            user.BannerImage = fileName;
            _userRepository.Update(user);
        }
        public async Task<Response<string>> PostCreated(string userId,string id)
        {
            var user = await _userRepository.GetFirstAsync(u => u.Id == userId);
            //user.Posts!.Add(id);
            _userRepository.Update(user);
            return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));
        }


        public async Task<Response<UserInfoGetResponse>> GetUserInfoForPost(string id, string sourceUserId)
        {
            UserInfoGetResponse dto = new();
            _User user = await _userRepository.GetFirstAsync(u => u.Id == id);

            dto.UserId = user.Id;
            dto.FirstName = user.FirstName;
            dto.LastName = user.LastName;
            dto.UserName = user.UserName;
            dto.ProfileImage = user.ProfileImage;
            dto.IsUserFollowing = user.Followers.Contains(sourceUserId);

            return await Task.FromResult(Response<UserInfoGetResponse>.Success(dto,ResponseStatus.Success));
        }

        public async Task<Response<GetCommunityOwnerDto>> GetCommunityOwner(string id)
        {

            _User user = await _userRepository.GetFirstAsync(u => u.Id == id);
            if (user != null)
            {
                GetCommunityOwnerDto dto = new();
                dto.OwnerId = user.Id;
                dto.Name = user.FirstName + ' ' + user.LastName;
                dto.ProfileImage = user.ProfileImage;
                return await Task.FromResult(Response<GetCommunityOwnerDto>.Success(dto, ResponseStatus.Success));
            }
            return await Task.FromResult(Response<GetCommunityOwnerDto>.Fail("Not found", ResponseStatus.NotFound));
        }

        public async Task<Response<UserInfoForCommentDto>> GetUserInfoForComment(string id)
        {
            _User user = await _userRepository.GetFirstAsync(u => u.Id == id);
            UserInfoForCommentDto dto = _mapper.Map<UserInfoForCommentDto>(user);
            return await Task.FromResult(Response<UserInfoForCommentDto>.Success(dto, ResponseStatus.Success));
        }

        public async Task<Response<List<string>>> GetUserFollowings(string id)
        {
            _User user = await _userRepository.GetFirstAsync(u => u.Id == id);
            if (user == null)
                return await Task.FromResult(Response<List<string>>.Fail("User Not found", ResponseStatus.BadRequest));

            return await Task.FromResult(Response<List<string>>.Success(user.Followings.ToList(), ResponseStatus.Success));

        }


        public async Task<Response<string>> AcceptFollowRequest(string id, string targetId)
        {
            try
            {
                _User user = await _userRepository.GetFirstAsync(u => u.Id == id);

                if (user != null)
                {
                    if (user.IncomingFollowRequests != null && user.IncomingFollowRequests.Contains(targetId))
                    {
                        _User targetUser = await _userRepository.GetFirstAsync(u => u.Id == targetId);
                        user.Followers!.Add(targetId);
                        targetUser.OutgoingFollowRequests.Remove(targetId);
                        targetUser.Followings!.Add(id);
                        user.IncomingFollowRequests.Remove(targetId);
                        
                        _userRepository.Update(user);
                        _userRepository.Update(targetUser);
                        return await Task.FromResult(Response<string>.Success("Successfully", ResponseStatus.Success));

                    }
                }

                return await Task.FromResult(Response<string>.Fail("User Not Found", ResponseStatus.NotFound));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", ResponseStatus.InitialError));
            }
        }

        public Task<Response<string>> DeclineFollowRequest(string id, string targetId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<List<GetUserByIdDto>>> GetUserList(UserIdListDto dto, int skip = 0, int take = 10)
        {
            try
            {
                DatabaseResponse response = await _userRepository.GetAllAsync(take, skip, u => dto.Ids.Contains(u.Id));
                List<GetUserByIdDto> userDtos = _mapper.Map<List<_User>, List<GetUserByIdDto>>(response.Data);

                return await Task.FromResult(Response<List<GetUserByIdDto>>.Success(userDtos, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<GetUserByIdDto>>.Fail($"Error occured {e}", ResponseStatus.InitialError));

            }
        }

        public async Task<Response<List<FollowingRequestDto>>> GetFollowerRequests(string userId, int skip = 0, int take = 10)
        {
            try
            {
                if (userId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<List<FollowingRequestDto>>.Fail("User Not Found", ResponseStatus.BadRequest));
                }

                _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);

                if (user != null)
                {
                    List<string> incomingRequests = user.IncomingFollowRequests?.ToList() ?? new List<string>();
                    DatabaseResponse incomingRequstUsers = await _userRepository.GetAllAsync(take, skip, u => incomingRequests.Contains(u.Id));

                    List<FollowingRequestDto> dtos = _mapper.Map<List<_User>, List<FollowingRequestDto>>(incomingRequstUsers.Data);
                  
                    return await Task.FromResult(Response<List<FollowingRequestDto>>.Success(dtos, ResponseStatus.Success));
                }

                return await Task.FromResult(Response<List<FollowingRequestDto>>.Fail("User Not Found", ResponseStatus.NotFound));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<FollowingRequestDto>>.Fail($"Error occured {e}", ResponseStatus.InitialError));
            }

        }
    }
    
}

