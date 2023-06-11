using System.Net.Http.Headers;
using System.Net.Http.Json;
using AutoMapper;
using DBHelper.Repository;
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
using _User = Topluluk.Services.User.Model.Entity.User;
using JsonSerializer = System.Text.Json.JsonSerializer;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.User.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFollowRepository _followRepository;
        private readonly IBlockedUserRepository _blockedUserRepository;
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly IRedisRepository _redisRepository;
        private readonly IUserFollowRequestRepository _followRequestRepository;
        public UserService(IRedisRepository redisRepository, IUserRepository userRepository, IBlockedUserRepository blockedUserRepository, IUserFollowRepository followRepository, IMapper mapper, IUserFollowRequestRepository followRequestRepository)
        {
            _redisRepository = redisRepository;
            _userRepository = userRepository;
            _followRepository = followRepository;
            _blockedUserRepository = blockedUserRepository;
            _mapper = mapper;
            _followRequestRepository = followRequestRepository;
            _client = new RestClient();
        }

        public async Task<Response<GetUserByIdDto>> GetUserById(string id, string userId)
        {
           _User? user = new();

                if (_redisRepository.IsConnected)
                {
                    string key = $"user_{userId}";

                    var cacheUser = await _redisRepository.GetValueAsync(key);

                    if (cacheUser.IsNullOrEmpty())
                    {
                        user = await _userRepository.GetFirstAsync(u => u.Id == userId);
                        await _redisRepository.SetValueAsync(key, user);
                    }
                    else
                    {
                        user = JsonSerializer.Deserialize<_User>(cacheUser);
                    }
                }
                else
                {
                    user = await _userRepository.GetFirstAsync(u => u.Id == userId);
                }
                
                GetUserByIdDto dto = _mapper.Map<GetUserByIdDto>(user);
                
                var isFollowingTask = _followRepository.AnyAsync(f => !f.IsDeleted && f.SourceId == id && f.TargetId == userId);
                var isFollowRequestSentTask = _followRequestRepository.AnyAsync(f => !f.IsDeleted && f.SourceId == id && f.TargetId == userId);
                var isFollowRequestReceivedTask = _followRequestRepository.AnyAsync(f => !f.IsDeleted && f.SourceId == userId && f.TargetId == id);
                var followingCountTask = _followRepository.Count(f => !f.IsDeleted &&  f.SourceId == userId);
                var followersCountTask = _followRepository.Count(f => !f.IsDeleted && f.TargetId == userId );
                var userCommunitiesRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/community/user-communities-count").AddQueryParameter("id",userId);
                var userCommunitiesTask = _client.ExecuteGetAsync<Response<int>>(userCommunitiesRequest);

                await Task.WhenAll(isFollowingTask,isFollowRequestSentTask,isFollowRequestReceivedTask, followingCountTask, followersCountTask, userCommunitiesTask);

                dto.IsFollowRequestSent = isFollowRequestSentTask.Result;
                dto.isFollowRequestReceived = isFollowRequestReceivedTask.Result;

                dto.IsFollowing = isFollowingTask.Result;
                dto.FollowingCount = followingCountTask.Result;
                dto.FollowersCount = followersCountTask.Result;
                var userCommunitiesResponse = userCommunitiesTask.Result;
                if (userCommunitiesResponse.IsSuccessful)
                {
                    dto.CommunityCount = userCommunitiesResponse.Data!.Data;
                }
                return Response<GetUserByIdDto>.Success(dto, ResponseStatus.Success);
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
                var isFollowRequestSentTask = _followRequestRepository.AnyAsync(f => !f.IsDeleted && f.SourceId == id && f.TargetId == user.Id);
                var isFollowRequestReceivedTask = _followRequestRepository.AnyAsync(f => !f.IsDeleted && f.SourceId == user.Id && f.TargetId == id);

                dto.IsFollowRequestSent = isFollowRequestSentTask.Result;
                dto.isFollowRequestReceived = isFollowRequestReceivedTask.Result;

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

                    _followRepository.DeleteByExpression(f=>f.SourceId == id || f.TargetId == id);
                    _followRequestRepository.DeleteByExpression(f=>f.SourceId == id || f.TargetId == id);
                    
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
            var users = _userRepository.GetListByExpressionPaginated(0, 10, u => u.IsDeleted == false && u.Id != userId);

            var followingUserIds = _followRepository.GetListByExpression(f => f.SourceId == userId);
    
            var filteredUsers = users.Where(u => !followingUserIds.Select(x => x.TargetId).ToList().Contains(u.Id)).ToList();

            var userDtos = _mapper.Map<List<UserSuggestionsDto>>(filteredUsers);

            return await Task.FromResult(Response<List<UserSuggestionsDto>>.Success(userDtos, ResponseStatus.Success));

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
                var followinCountTask =  _followRepository.Count(u => !u.IsDeleted && u.SourceId == id);
                var followersCountTask =  _followRepository.Count(u => !u.IsDeleted && u.TargetId == id);
                await Task.WhenAll(followersCountTask, followersCountTask);
                
                dto.FollowingsCount = followinCountTask.Result;
                dto.FollowersCount = followersCountTask.Result;
                
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
                
                // It was true at first.
                if (user.IsPrivate != false)
                    return await Task.FromResult(Response<string>.Success(
                        $"Privacy status Successfully updated to {user.IsPrivate}", ResponseStatus.Success));
                
                var followRequests = await _followRequestRepository.GetListByExpressionAsync(f => f.TargetId == userId);
                
                var followDocuments = followRequests.Select(followRequest => 
                        new UserFollow() { SourceId = followRequest.SourceId, TargetId = followRequest.TargetId }).ToList();
                if (followDocuments != null && followDocuments.Count > 0)
                {
                    await _followRepository.InsertManyAsync(followDocuments);
                    _followRequestRepository.DeleteByExpression(x => followRequests.Select(f =>f.TargetId).ToList().Contains(x.TargetId));
                }
          
                


                return await Task.FromResult(Response<string>.Success($"Privacy status Successfully updated to {user.IsPrivate}", ResponseStatus.Success));

            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Error occured {e}", ResponseStatus.InitialError));

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
                        return Response<NoContent>.Fail("Email already taken!", ResponseStatus.EmailInUse);
                    }
                }
                // Http request to auth service for change username and email

                UserCredentialUpdateDto credentialDto = new();
                credentialDto.UserName = userDto.UserName;
                credentialDto.Email = userDto.Email;

                var _request = new RestRequest(ServiceConstants.API_GATEWAY + "/authentication/update-profile").AddHeader("Authorization", token).AddBody(credentialDto);
                await _client.ExecutePostAsync<Response<NoContent>>(_request);

                user.FirstName = userDto.FirstName.IsNullOrEmpty() ? user.FirstName : userDto.FirstName;
                user.LastName = userDto.LastName.IsNullOrEmpty() ?  user.LastName : userDto.LastName;
                user.UserName =  userDto.UserName.IsNullOrEmpty() ? user.UserName : userDto.UserName;
                user.Email = userDto.Email.IsNullOrEmpty() ? user.Email : userDto.Email;
                user.Gender = userDto.Gender;
                user.Bio = userDto.Bio.IsNullOrEmpty() ? user.Bio : userDto.Bio;
                user.BirthdayDate = userDto.BirthdayDate;
                user.Title = userDto.Title.IsNullOrEmpty() ? user.Title : userDto.Title;
                DatabaseResponse response = _userRepository.Update(user);

                if (response.IsSuccess)
                {
                    return Response<NoContent>.Success( ResponseStatus.Success);
                }


                return Response<NoContent>.Fail("Update failed", ResponseStatus.InitialError);
            }
            catch (Exception e)
            {
                return Response<NoContent>.Fail(e.ToString(), ResponseStatus.InitialError);
            }
        }


        
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ For Http calls coming from other services @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\


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

        public async Task<Response<List<GetUserByIdDto>>> GetUserList(IdList dto, int skip = 0, int take = 10)
        {
            try
            {
                List<_User> users = new();

                if (_redisRepository.IsConnected)
                {
                    List<string> keys = dto.ids.Select(id => $"user_{id}").ToList();
                    var cachedUsers = await _redisRepository.GetAllAsync(keys);

                    List<string> missingRedisKeys = cachedUsers.Where(doc => string.IsNullOrEmpty(doc.Value)).Select(doc => doc.Key).ToList();
                    List<string> missingIds = missingRedisKeys.Select(key => key.Substring(5)).ToList();

                    if (missingRedisKeys.Any())
                    {
                        var missingUsers = _userRepository.GetListByExpression(u => missingIds.Contains(u.Id));
                        foreach (var user in missingUsers)
                        {
                             _redisRepository.SetValueAsync($"user_{user.Id}", JsonSerializer.Serialize(user));
                        }
                        users.AddRange(missingUsers);
                    }

                    users.AddRange(cachedUsers.Where(doc => !string.IsNullOrEmpty(doc.Value)).Select(doc => JsonConvert.DeserializeObject<_User>(doc.Value)));
                }
                else
                {
                    users = _userRepository.GetListByExpressionPaginated(skip, take, u => dto.ids.Contains(u.Id));
                }

                List<GetUserByIdDto> userDtos = _mapper.Map<List<_User>, List<GetUserByIdDto>>(users);

                return Response<List<GetUserByIdDto>>.Success(userDtos, ResponseStatus.Success);
            }
            catch (Exception e)
            {
                return Response<List<GetUserByIdDto>>.Fail($"Error occured {e}", ResponseStatus.InitialError);
            }
        }
  
        public async Task<Response<List<FollowingUserDto>>> SearchInFollowings(string id, string userId, string text, int skip = 0, int take = 10)
        {
            try
            {
                if (userId.IsNullOrEmpty())
                {
                    return Response<List<FollowingUserDto>>.Fail("User Not Found", ResponseStatus.BadRequest);
                }

                _User? user = await _userRepository.GetFirstAsync(u => u.Id == userId);

                if (user == null)
                {
                    return Response<List<FollowingUserDto>>.Fail("User not found", ResponseStatus.NotFound);
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
                return Response<List<FollowingUserDto>>.Success(followingUserDtos, ResponseStatus.Success);
            }
            catch (Exception e)
            {
                return Response<List<FollowingUserDto>>.Fail($"Some error occurreed : {e}", ResponseStatus.InitialError);
            }
        }

        
    }

}

