using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AutoMapper;
using DBHelper.Repository;
using DBHelper.Repository.Redis;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
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
using _User = Topluluk.Services.User.Model.Entity.User;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.User.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFollowRepository _followRepository;
        private readonly IBlockedUserRepository _blockedUserRepository;
        private readonly ICapPublisher _capPublisher;
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly IRedisRepository _redisRepository;
        public UserService(IRedisRepository redisRepository, IUserRepository userRepository, IBlockedUserRepository blockedUserRepository, IUserFollowRepository followRepository, ICapPublisher capPublisher, IMapper mapper)
        {
            _redisRepository = redisRepository;
            _userRepository = userRepository;
            _followRepository = followRepository;
            _blockedUserRepository = blockedUserRepository;
            _capPublisher = capPublisher;
            _mapper = mapper;
            _client = new RestClient();
        }

        public async Task<Response<GetUserByIdDto>> GetUserById(string id, string userId)
        {
            try
            {
                _User? user = await _userRepository.GetFirstAsync(u => u.Id == userId);
                if (user == null)
                {
                    return await Task.FromResult(Response<GetUserByIdDto>.Fail("User Not Found",
                        ResponseStatus.NotFound));
                }

                if (_redisRepository.IsConnected)
                {
                    string key = $"user_{user.Id}";
                    var userObject = new 
                        { Id = user.Id,FirstName=user.FirstName,LastName=user.LastName,UserName = user.UserName,
                            ProfileImage=user.ProfileImage,Gender=user.Gender};
                    
                    var userJson = JsonSerializer.Serialize(userObject);
                    await _redisRepository.SetValueAsync(key, userJson);
                }
                GetUserByIdDto dto = _mapper.Map<GetUserByIdDto>(user);
                dto.IsFollowRequestSent = user.IncomingFollowRequests!.Contains(id);
                dto.IsFollowRequested = user.OutgoingFollowRequests!.Contains(id);

                dto.IsFollowing = await _followRepository.AnyAsync(f => f.SourceId == id && f.TargetId == userId);
                dto.FollowingCount = await _followRepository.Count(f => f.SourceId == userId);
                dto.FollowersCount = await _followRepository.Count(f => f.TargetId == userId);
                return await Task.FromResult(Response<GetUserByIdDto>.Success(dto, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<GetUserByIdDto>.Fail($"Some error occured: {e}",
                    ResponseStatus.InitialError));
            }

        }

        public async Task<Response<GetUserByIdDto>> GetUserByUserName(string id, string userName)
        {
            try
            {
                _User? user = await _userRepository.GetFirstAsync(u => u.UserName == userName);
                if (user == null)
                {
                    return await Task.FromResult(Response<GetUserByIdDto>.Fail("User Not Found",
                        ResponseStatus.NotFound));
                }

                string key = $"user_{user.Id}";
                var userObject = new { Id = user.Id,FirstName=user.FirstName,LastName=user.LastName,ProfileImage=user.ProfileImage,Gender=user.Gender};
                var userJson = JsonSerializer.Serialize(userObject);
                await _redisRepository.SetValueAsync(key, userJson);
                GetUserByIdDto dto = _mapper.Map<GetUserByIdDto>(user);
                dto.IsFollowRequestSent = user.IncomingFollowRequests!.Contains(id);
                dto.IsFollowRequested = user.OutgoingFollowRequests!.Contains(id);

                dto.IsFollowing = await _followRepository.AnyAsync(f => f.SourceId == id && f.TargetId == id);
                dto.FollowingCount = await _followRepository.Count(f => f.SourceId == id);
                dto.FollowersCount = await _followRepository.Count(f => f.TargetId == id);
                return await Task.FromResult(Response<GetUserByIdDto>.Success(dto, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<GetUserByIdDto>.Fail($"Some error occured: {e}",
                    ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> InsertUser(UserInsertDto userInfo)
        {
            _User user = _mapper.Map<_User>(userInfo);
            DatabaseResponse response = await _userRepository.InsertAsync(user);

            return await Task.FromResult(Response<string>.Success(response.Data, ResponseStatus.Success));
        }

        public async Task<Response<string>> DeleteUserById(string id, string token, UserDeleteDto dto)
        {
            try
            {
                if (id.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<string>.Fail("Bad Request", ResponseStatus.BadRequest));
                }

                bool isUserExist = await _userRepository.AnyAsync(u => u.Id == id);

                if (isUserExist == false)
                {
                    return await Task.FromResult(Response<string>.Fail("User not found", ResponseStatus.NotFound));
                }

                // Topluluğu var mı kontrol et, varsa fail dön
                var checkCommunitiesRequest = new RestRequest("https://localhost:7149/api/community/check-is-user-community-owner")
                                                  .AddHeader("Authorization", token);
                var checkCommunitiesResponse = await _client.ExecuteGetAsync<Response<bool>>(checkCommunitiesRequest);

                if (checkCommunitiesResponse.Data!.Data == true)
                {
                    return await Task.FromResult(Response<string>.Fail("You have to delete your community first", ResponseStatus.CommunityOwnerExist));

                }
                // Sahibi olduğu topluluk yoksa
                else
                {

                    // Posts and PostComments will be deleted.
                    var deletePostsRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/post/delete-posts").AddHeader("Authorization", token);
                    var deletePostsResponse = await _client.ExecutePostAsync<Response<bool>>(deletePostsRequest);

                    if (deletePostsResponse.Data.IsSuccess == false) throw new Exception();

                    // Event, EventComments will be deleted.

                    // Delete User.
                    DatabaseResponse response = _userRepository.DeleteById(id);
                    var userDeleteRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/authentication/delete").AddBody(dto).AddHeader("Authorization", token);
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
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> FollowUser(string userId, UserFollowDto userFollowInfo)
        {

            try
            {
                _User sourceUser = await _userRepository.GetFirstAsync(u => u.Id == userId);

                if (sourceUser == null)
                    return await Task.FromResult(Response<string>.Fail("User not found", ResponseStatus.NotFound));
                
                if (userId == userFollowInfo.TargetId)
                    return await Task.FromResult(Response<string>.Fail("You can not follow yourself",
                        ResponseStatus.BadRequest));
                
                _User targetUser = await _userRepository.GetFirstAsync(u => u.Id == userFollowInfo.TargetId);
                bool isFollowing =
                    await _followRepository.AnyAsync(f => f.SourceId == userId && f.TargetId == userFollowInfo.TargetId);

                if (isFollowing)
                    return await Task.FromResult(Response<string>.Success("Already following", ResponseStatus.Success));
                
                if (targetUser.IsPrivate)
                {
                    // todo: Refactor here.
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
                    UserFollow userFollow = new() { SourceId = userId, TargetId = userFollowInfo.TargetId };
                    await _followRepository.InsertAsync(userFollow);

                    return await Task.FromResult(Response<string>.Success("Successfully followed!", ResponseStatus.Success));
                }

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> UnFollowUser(string userId, UserFollowDto userFollowInfo)
        {
            UserFollow? isSourceFollowingTarget =
                await _followRepository.GetFirstAsync(f => f.SourceId == userId && f.TargetId == userFollowInfo.TargetId);

            if (isSourceFollowingTarget == null)
            {
                return await Task.FromResult(Response<string>.Fail("Failed", ResponseStatus.Failed));
            }

            _followRepository.DeleteCompletely(isSourceFollowingTarget.Id);

            return await Task.FromResult(Response<string>.Success("Successfully unfollowed!", ResponseStatus.Success));
        }

        public async Task<Response<string>> RemoveUserFromFollowers(string userId, UserFollowDto userFollowInfo)
        {
            try
            {
                UserFollow? userFollow = await _followRepository.GetFirstAsync(f => f.SourceId == userFollowInfo.TargetId && f.TargetId == userId);

                if (userFollow == null)
                {
                    return await Task.FromResult(Response<string>.Fail("Failed", ResponseStatus.Failed));
                }

                _followRepository.DeleteCompletely(userFollow.Id);
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

            if (sourceUser == null || targetUser == null)
            {
                return await Task.FromResult(Response<string>.Fail("User not found", ResponseStatus.NotFound));
            }

            await _blockedUserRepository.InsertAsync(new() { SourceId = sourceId, TargetId = targetId });

            UserFollow? isSourceUserFollowsTarget =
                await _followRepository.GetFirstAsync(f => f.SourceId == sourceId && f.TargetId == targetId);
            UserFollow? isTargetUserFollowsSource =
                await _followRepository.GetFirstAsync(f => f.SourceId == targetId && f.TargetId == sourceId);

            if (isSourceUserFollowsTarget != null)
            {
                _followRepository.DeleteCompletely(isSourceUserFollowsTarget.Id);
            }

            if (isTargetUserFollowsSource != null)
            {
                _followRepository.DeleteCompletely(isTargetUserFollowsSource.Id);
            }

            return await Task.FromResult(Response<string>.Success("User blocked successfully.", ResponseStatus.Success));
        }

        public async Task<Response<List<UserSuggestionsDto>>> GetUserSuggestions(string userId, int limit = 5)
        {
            /* id, image, firstName, lastName, userName, isPrivate(direkt follow edip edememe durumu için gerekli.)
             */

            _User currentUser = await _userRepository.GetFirstAsync(u => u.Id == userId);
            List<_User> response = new();
            // Sorgulanan kullanıcı bizi bloklamamış olmalı
            // Bizim sorguladığımız kullanıcıyı bloklamamış olmamız lazım.
             response =  _userRepository.GetListByExpressionPaginated(limit, 0, u => u.IsDeleted == false && u.Id != userId);
            
            //u.BlockedUsers!.Contains("") == false
            //&& currentUser.BlockedUsers!.Contains(u.Id) == false
            List<UserSuggestionsDto> userSuggestions = _mapper.Map<List<UserSuggestionsDto>>(response);

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

            DatabaseResponse response = await _userRepository.GetAllAsync(take, skip, u => u.Id != userId && ((u.FirstName.ToLower() + " " +
                                                                                     u.LastName.ToLower()).Contains(text.ToLower()) ||
                                                                                     u.UserName.ToLower().Contains(text.ToLower()))
                                                                                     );



            List<UserSearchResponseDto> users = _mapper.Map<List<_User>, List<UserSearchResponseDto>>(response.Data);

            return await Task.FromResult(Response<List<UserSearchResponseDto>>.Success(users, ResponseStatus.Success));
        }

        public async Task<Response<string>> ChangeProfileImage(string userName, IFormFileCollection files, CancellationToken cancellationToken)
        {

            try
            {
                if (files == null || files.Count == 0)
                {
                    return await Task.FromResult(Response<string>.Fail("Atleast Need 1 image",
                        ResponseStatus.BadRequest));
                }
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
    
                        NameObject nameObject = new() { Name = user.ProfileImage};
                        var request = new RestRequest(ServiceConstants.API_GATEWAY + "/file/delete-user-image").AddBody(nameObject);
                        var response1 = await _client.ExecutePostAsync<Response<string>>(request, cancellationToken: cancellationToken);
                    }

                    var response = await client.PostAsync("https://localhost:7165/file/upload-user-image", content);

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
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred: {e}",ResponseStatus.InitialError));
            }
        }

        public async Task<Response<NoContent>> DeleteProfileImage(string userId)
        {
            try
            {
                _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
                if (user == null)
                    return await Task.FromResult(Response<NoContent>.Fail("User Not Found", ResponseStatus.NotFound));

                if (user.ProfileImage == null)
                    return await Task.FromResult(Response<NoContent>.Success(ResponseStatus.Success));
                NameObject nameObject = new() { Name = user.ProfileImage};
                var request = new RestRequest(ServiceConstants.API_GATEWAY + "/file/delete-user-image").AddBody(nameObject);
                var response = await _client.ExecutePostAsync<Response<string>>(request);

                if (!response.IsSuccessful || !response.Data!.IsSuccess)
                    return await Task.FromResult(Response<NoContent>.Fail("Failed", ResponseStatus.Failed));
                
                user.ProfileImage = null;
                _userRepository.Update(user);
                return await Task.FromResult(Response<NoContent>.Success(ResponseStatus.Success));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<NoContent>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> ChangeBannerImage(string userId, UserChangeBannerDto changeBannerDto)
        {
            try
            {
                if (changeBannerDto.File == null)
                {
                    return await Task.FromResult(Response<string>.Fail("Atleast Need 1 image",
                        ResponseStatus.BadRequest));
                }
            
                _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
            
                if (user == null)
                {
                    return await Task.FromResult(Response<string>.Fail("User Not Found", ResponseStatus.NotFound));
                }
            
                Response<string>? responseData = new();
                byte[] imageBytes;

                using (var stream = new MemoryStream())
                {
                    changeBannerDto.File.CopyTo(stream);
                    imageBytes = stream.ToArray();
                }

                using (var client = new HttpClient())
                {
                    var content = new MultipartFormDataContent();
                    var imageContent = new ByteArrayContent(imageBytes);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg"); 
                    content.Add(imageContent, "File", changeBannerDto.File.FileName); 

                    if (user.BannerImage != null)
                    {

                        NameObject nameObject = new() { Name = user.ProfileImage};
                        var request = new RestRequest(ServiceConstants.API_GATEWAY + "/file/delete-user-image").AddBody(nameObject);
                        var response1 = await _client.ExecutePostAsync<Response<string>>(request);
                    }

                    var response = await client.PostAsync("https://localhost:7165/file/upload-user-banner", content);

                    if (response.IsSuccessStatusCode)
                    {
                        responseData = await response.Content.ReadFromJsonAsync<Response<string>>();
                        if (responseData != null)
                        {
                            user.BannerImage = responseData.Data;
                            _userRepository.Update(user);
                            return await Task.FromResult(Response<string>.Success($"Image changed with {user.BannerImage}", ResponseStatus.Success));
                        }

                        throw new Exception($"{typeof(UserService)} exception, IsSuccessStatusCode=true, responseData=null");
                    }
                    else
                    {
                        return await Task.FromResult(Response<string>.Fail("Failed while uploading image with http client", ResponseStatus.InitialError));
                    }
                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }

        public async Task<Response<NoContent>> DeleteBannerImage(string userId)
        {
            try
            {
                _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
                if (user == null)
                    return await Task.FromResult(Response<NoContent>.Fail("User Not Found", ResponseStatus.NotFound));

                if (user.BannerImage == null)
                    return await Task.FromResult(Response<NoContent>.Success(ResponseStatus.Success));
                NameObject nameObject = new() { Name = user.BannerImage};
                var request = new RestRequest(ServiceConstants.API_GATEWAY + "/file/delete-user-banner").AddBody(nameObject);
                var response = await _client.ExecutePostAsync<Response<string>>(request);

                if (!response.IsSuccessful || !response.Data!.IsSuccess)
                    return await Task.FromResult(Response<NoContent>.Fail("Failed", ResponseStatus.Failed));
                
                user.BannerImage = null;
                _userRepository.Update(user);
                return await Task.FromResult(Response<NoContent>.Success(ResponseStatus.Success));
            }
            catch (Exception e)
            {

                return await Task.FromResult(Response<NoContent>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }
        }

        // todo FollowingRequest leri kullanıcı adı, ad, soyad, id, kullanıcı resmi ve istek attığı tarih ile dön.
        public async Task<Response<GetUserAfterLoginDto>> GetUserAfterLogin(string id)
        {
            try
            {
                _User user = await _userRepository.GetFirstAsync(u => u.Id == id);
                if (user == null)
                {
                    return await Task.FromResult(
                        Response<GetUserAfterLoginDto>.Fail("User Not Found", ResponseStatus.NotFound));
                }
                
                GetUserAfterLoginDto dto = new();
                dto = _mapper.Map<GetUserAfterLoginDto>(user);
                dto.FollowingsCount = await _followRepository.Count(u => u.SourceId == id);
                dto.FollowersCount = await _followRepository.Count(u => u.TargetId == id);
                return await Task.FromResult(Response<GetUserAfterLoginDto>.Success(dto, ResponseStatus.Success));
            }
            catch(Exception e)
            {
                return await Task.FromResult(Response<GetUserAfterLoginDto>.Fail($"Some error occurred: {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> PrivacyChange(string userId, UserPrivacyChangeDto dto)
        {
            try
            {
                if (userId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<string>.Fail("Bad Request", ResponseStatus.BadRequest));
                }

                _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);

                if (user == null)
                {
                    return await Task.FromResult(Response<string>.Fail("UnAuthorized", ResponseStatus.NotAuthenticated));

                }

                user.IsPrivate = dto.IsPrivate;
                DatabaseResponse response = _userRepository.Update(user);

                if (response.IsSuccess == false)
                {
                    return await Task.FromResult(Response<string>.Fail("Update failed", ResponseStatus.BadRequest));
                }

                // Kullanıcı profilini açığa çekmişse incomingFollowRequestleri kabulet. Usera followings olarak ekle
                // incomingRequestlerdeki id lere sahip userların outgoingFollowRequestleri sil, usera followers olarak ekle.
                if (user.IsPrivate == false)
                {
                    if (user.IncomingFollowRequests != null && user.IncomingFollowRequests.Count > 0)
                    {

                        foreach (var _userId in user.IncomingFollowRequests.ToList())
                        {
                            await _followRepository.InsertAsync(new() { SourceId = _userId, TargetId = userId });
                        }
                    }
                }


                return await Task.FromResult(Response<string>.Success($"Privacy status Successfully updated to {user.IsPrivate}", ResponseStatus.Success));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", ResponseStatus.InitialError));

            }
        }

        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ For Http calls coming from other services @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\



        public async Task<Response<string>> PostCreated(string userId, string id)
        {
            var user = await _userRepository.GetFirstAsync(u => u.Id == userId);
            //user.Posts!.Add(id);
            _userRepository.Update(user);
            return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));
        }


        public async Task<Response<UserInfoForPostDto>> GetUserInfoForPost(string id, string sourceUserId)
        {
            try
            {
                UserInfoForPostDto dto = new();
                _User user = new();
                if (_redisRepository.IsConnected)
                {
                    user = await _redisRepository.GetOrNullAsync<_User>($"user_{id}") 
                                 ?? await _userRepository.GetFirstAsync(u => u.Id == id);
                }
                else
                {
                    user = await _userRepository.GetFirstAsync(u => u.Id == id);
                }

                dto = _mapper.Map<UserInfoForPostDto>(user);
                dto.UserId = user.Id;
                dto.IsUserFollowing = await _followRepository.AnyAsync(f => f.SourceId == sourceUserId && f.TargetId == id);
                return await Task.FromResult(Response<UserInfoForPostDto>.Success(dto, ResponseStatus.Success));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<UserInfoForPostDto>.Fail($"Error occured {e}", ResponseStatus.InitialError));

            }
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
            List<string> followingIds = new();
            var followings = _followRepository.GetListByExpression(f => f.SourceId == id);
            foreach (var follow in followings)
            {
                followingIds.Add(follow.TargetId);
            }
            return await Task.FromResult(Response<List<string>>.Success(followingIds, ResponseStatus.Success));

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
                        //    user.Followers!.Add(targetId);
                        targetUser.OutgoingFollowRequests.Remove(targetId);
                        //       targetUser.Followings!.Add(id);
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

        public async Task<Response<List<UserFollowRequestDto>>> GetFollowerRequests(string id, string userId, int skip = 0, int take = 10)
        {
            try
            {
                if (!id.Equals(userId))
                {
                    return await Task.FromResult(Response<List<UserFollowRequestDto>>.Fail("",
                        ResponseStatus.Unauthorized));
                }

                _User? user = await _userRepository.GetFirstAsync(u => u.Id == userId);

                List<string> requestIds = user.IncomingFollowRequests.ToList();

                List<_User> users = _userRepository.GetListByExpression(u => requestIds.Contains(u.Id));
                var dto = _mapper.Map<List<_User>, List<UserFollowRequestDto>>(users);

                return await Task.FromResult(Response<List<UserFollowRequestDto>>.Success(dto, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<UserFollowRequestDto>>.Fail($"Some error occurred: {e}",
                    ResponseStatus.InitialError));
            }

        }

        // Takip edilen kullanıcıları ve o kullanıcıların bilgilerini getireceğiz
        public async Task<Response<List<FollowingUserDto>>> GetFollowingUsers(string id, string userId, int skip = 0, int take = 10)
        {
            try
            {
                if (userId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<List<FollowingUserDto>>.Fail("Bad Request", ResponseStatus.BadRequest));
                }

                _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
                if (user == null)
                {
                    return await Task.FromResult(Response<List<FollowingUserDto>>.Fail("", ResponseStatus.NotFound));
                }
                // Target Id leri elimde. Bu target id ler ile kullanıcı bilgilerini alıcaz.
                DatabaseResponse followingsResponse = await _followRepository.GetAllAsync(take, skip, f => f.SourceId == userId);
                List<string> x = new();
                foreach (var y in followingsResponse.Data)
                {
                    x.Add(y.TargetId);
                }
                DatabaseResponse followingUsers = await _userRepository.GetAllAsync(take, skip, fu => x.Contains(fu.Id));
                List<FollowingUserDto> dtos = _mapper.Map<List<_User>, List<FollowingUserDto>>(followingUsers.Data);

                return await Task.FromResult(Response<List<FollowingUserDto>>.Success(dtos, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<FollowingUserDto>>.Fail($"Some error occurred: {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<List<FollowerUserDto>>> GetFollowerUsers(string id, string userId, int skip = 0, int take = 10)
        {
            try
            {
                if (id.IsNullOrEmpty() || userId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<List<FollowerUserDto>>.Fail("", ResponseStatus.BadRequest));
                }

                _User? user = await _userRepository.GetFirstAsync(u => u.Id == userId);

                if (user == null)
                {
                    return await Task.FromResult(Response<List<FollowerUserDto>>.Fail($"User Not Found!", ResponseStatus.NotFound));
                }

                List<UserFollow> followers = _followRepository.GetListByExpressionPaginated(skip,take,f => f.TargetId == userId);

                List<_User> users = _userRepository.GetListByExpressionPaginated(skip,take, u => followers.Any(f => f.SourceId == u.Id));
                List<FollowerUserDto> followersDto = _mapper.Map<List<_User>, List<FollowerUserDto>>(users);

                return await Task.FromResult(Response<List<FollowerUserDto>>.Success(followersDto, ResponseStatus.Success));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<FollowerUserDto>>.Fail($"Some error occurred: {e}", ResponseStatus.InitialError));

            }
        }

        public async Task<Response<NoContent>> UpdateProfile(string userId, string token, UserUpdateProfileDto userDto)
        {

            try
            {
                if (userId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<NoContent>.Fail("", ResponseStatus.Unauthorized));
                }


                _User? user = await _userRepository.GetFirstAsync(u => u.Id == userId);

                if (user == null)
                {
                    return await Task.FromResult(Response<NoContent>.Fail("User not found", ResponseStatus.NotFound));
                }

                if (user.Id != userId)
                {
                    return await Task.FromResult(Response<NoContent>.Fail("", ResponseStatus.Unauthorized));
                }


                if (user.UserName != userDto.UserName)
                {
                    var result = await _userRepository.CheckIsUsernameUnique(userDto.UserName);
                    if (result == true)
                    {
                        return await Task.FromResult(Response<NoContent>.Fail("UserName already taken!", ResponseStatus.UsernameInUse));
                    }
                }

                if (user.Email != userDto.Email)
                {
                    var result = await _userRepository.CheckIsUsernameUnique(userDto.Email);
                    if (result == true)
                    {
                        return await Task.FromResult(Response<NoContent>.Fail("Email already taken!", ResponseStatus.EmailInUse));
                    }
                }
                // Http request to auth service for change username and email

                UserCredentialUpdateDto credentialDto = new();
                credentialDto.UserName = userDto.UserName;
                credentialDto.Email = userDto.Email;

                var _request = new RestRequest(ServiceConstants.API_GATEWAY + "/authentication/update-profile").AddHeader("Authorization", token).AddBody(credentialDto);
                var _response = await _client.ExecutePostAsync<Response<NoContent>>(_request);

                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.UserName = userDto.UserName;
                user.Email = userDto.Email;
                user.Gender = userDto.Gender;
                user.Bio = userDto.Bio;
                user.BirthdayDate = userDto.BirthdayDate;

                DatabaseResponse response = _userRepository.Update(user);

                if (response.IsSuccess == true)
                {
                    return await Task.FromResult(Response<NoContent>.Success(null, ResponseStatus.Success));
                }


                return await Task.FromResult(Response<NoContent>.Fail("Update failed", ResponseStatus.InitialError));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<NoContent>.Fail($"Some error occurred: {e}", ResponseStatus.InitialError));
            }
        }

        public async Task<Response<List<FollowingUserDto>>> SearchInFollowings(string id, string userId, string text, int skip = 0, int take = 10)
        {
            try
            {
                if (userId.IsNullOrEmpty())
                {
                    return await Task.FromResult(Response<List<FollowingUserDto>>.Fail("User Not Found", ResponseStatus.BadRequest));
                }

                _User? user = await _userRepository.GetFirstAsync(u => u.Id == userId);

                if (user == null)
                {
                    return await Task.FromResult(Response<List<FollowingUserDto>>.Fail("User not found", ResponseStatus.NotFound));
                }
                // fixle true kısmını

                var response = _followRepository.GetListByExpressionPaginated(skip, take, f => f.SourceId == userId);
                List<string> userIds = new();
                foreach (var userFollow in response)
                {
                    userIds.Add(userFollow.TargetId);
                }
                var followingUsers =
                    _userRepository.GetListByExpressionPaginated(skip, take, u => userIds.Contains(u.Id) 
                        && ((u.FirstName.ToLower() + " " + u.LastName.ToLower()).Contains(text.ToLower()) || u.UserName.Contains(text.ToLower())) );
                List<FollowingUserDto> followingUserDtos =
                    _mapper.Map<List<_User>, List<FollowingUserDto>>(followingUsers);
                /*DatabaseResponse response = await _userRepository.GetAllAsync(take, skip, u =>  true
                && ((u.FirstName.ToLower() + " " + u.LastName.ToLower()).Contains(text.ToLower()) || u.UserName.Contains(text.ToLower())));
                ;*/
                return await Task.FromResult(Response<List<FollowingUserDto>>.Success(followingUserDtos, ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<List<FollowingUserDto>>.Fail($"Some error occurreed : {e}", ResponseStatus.InitialError));
            }
        }

        
    }

}

