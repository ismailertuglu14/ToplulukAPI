using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Topluluk.Services.User.Data.Interface;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Services.User.Services.Interface;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;
using Topluluk.Shared.Helper;
using _User = Topluluk.Services.User.Model.Entity.User;

namespace Topluluk.Services.User.Services.Implementation
{
	public class UserService : IUserService
	{
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

		public UserService(IUserRepository userRepository, IMapper mapper)
		{
            _userRepository = userRepository;
            _mapper = mapper;
		}

        public async Task<Response<string>> GetUserById(string id)
        {
            DatabaseResponse response = _userRepository.GetById(id);

            if(response.Data != null)
            {
                return await Task.FromResult(Response<string>.Success(JsonConvert.SerializeObject(response,Formatting.None), ResponseStatus.Success));
            }
            return await Task.FromResult(Response<string>.Fail("", ResponseStatus.NotFound));

        }

        public async Task<Response<string>> InsertUser(UserInsertDto userInfo)
        {
            _User user = _mapper.Map<_User>(userInfo);
            DatabaseResponse response = await _userRepository.InsertAsync(user);

            return await Task.FromResult(Response<string>.Success(response.Data,ResponseStatus.Success));
        }

        public Task<Response<string>> DeleteUserById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<string>> FollowUser(UserFollowDto userFollowInfo)
        {

            _User sourceUser = await _userRepository.GetFirstAsync(u => u.Id == userFollowInfo.SourceId);
            _User targetUser = await _userRepository.GetFirstAsync(u => u.Id == userFollowInfo.TargetId);

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
           // _User sourceUser = await _userRepository.GetFirstAsync(u => u.Id == userInfo.SourceId);
           // _User targetUser = await _userRepository.GetFirstAsync(u => u.Id == userInfo.TargetId);
           throw new NotImplementedException();
        }

        public Task<Response<string>> GetUserSuggestions(int limit = 4)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> SearchUser(string text, int skip = 0, int take = 5)
        {
            throw new NotImplementedException();
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

        // For Http calls coming from other services
        public async Task<Response<string>> UpdateCommunities(string userId, string communityId)
        {
            _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
            user.Communities?.Add(communityId);
            _userRepository.Update(user);
            return await Task.FromResult(Response<string>.Success("Success", ResponseStatus.Success));
        }
    }
}

