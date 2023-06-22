using System.Net.Http.Headers;
using System.Net.Http.Json;
using DBHelper.Repository;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Topluluk.Services.User.Data.Interface;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Services.User.Services.Interface;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Exceptions;
using _User = Topluluk.Services.User.Model.Entity.User;
using JsonSerializer = System.Text.Json.JsonSerializer;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;
namespace Topluluk.Services.User.Services.Implementation;

public class ImageService : IImageService
{
    private readonly IUserRepository _userRepository;
    private readonly RestClient _client;
    private readonly IRedisRepository _redisRepository;

    public ImageService(IUserRepository userRepository, IRedisRepository redisRepository)
    {
        _userRepository = userRepository;
        _redisRepository = redisRepository;
        _client = new RestClient();
    }


    public async Task<Response<string>> ChangeBannerImage(string userId, UserChangeBannerDto changeBannerDto)
    {
        if (changeBannerDto.File == null)
        {
            return Response<string>.Fail("Atleast Need 1 image",
                ResponseStatus.BadRequest);
        }
                
        _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
                
        if (user == null)
        {
            return Response<string>.Fail("User Not Found", ResponseStatus.NotFound);
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

            if (!response.IsSuccessStatusCode)
            {
                return Response<string>.Fail("Failed while uploading image with http client",
                    ResponseStatus.InitialError);
            }

            responseData = await response.Content.ReadFromJsonAsync<Response<string>>();

            user.BannerImage = responseData!.Data;
            bool updateSuccess = _userRepository.Update(user).IsSuccess;
                        

            if (updateSuccess && _redisRepository.IsConnected)
            {
                await _redisRepository.SetValueAsync($"user_{user.Id}",user);
            }

            return Response<string>.Success($"Image changed with {user.BannerImage}", ResponseStatus.Success);
        }    
    }

    public async Task<Response<NoContent>> DeleteBannerImage(string userId)
    {
        _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User Not Found");

        if (user.BannerImage == null)
            return Response<NoContent>.Success(ResponseStatus.Success);
            
        NameObject nameObject = new() { Name = user.BannerImage};
        var request = new RestRequest(ServiceConstants.API_GATEWAY + "/file/delete-user-banner").AddBody(nameObject);
        var response = await _client.ExecutePostAsync<Response<string>>(request);

        if (!response.IsSuccessful || !response.Data!.IsSuccess)
            return Response<NoContent>.Fail("Failed", ResponseStatus.Failed);
                
        user.BannerImage = null;
        _userRepository.Update(user);
        await _redisRepository.SetValueAsync($"user_{user.Id}",user);

        return Response<NoContent>.Success(ResponseStatus.Success);
    }
    public async Task<Response<string>> ChangeProfileImage(string userName, IFormFileCollection files, CancellationToken cancellationToken)
    {

        if (files == null || files.Count == 0)
        {
            return Response<string>.Fail("Atleast Need 1 image",
                ResponseStatus.BadRequest);
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
                await _client.ExecutePostAsync<Response<string>>(request, cancellationToken: cancellationToken);
            }

            var response = await client.PostAsync("https://localhost:7165/file/upload-user-image", content);

            if (!response.IsSuccessStatusCode)
            {
                // Resim yükleme işlemi başarısız
                return Response<string>.Fail("Failed while uploading image with http client",
                    ResponseStatus.InitialError);
            }

            responseData = await response.Content.ReadFromJsonAsync<Response<List<string>>>(cancellationToken: cancellationToken);
            if (responseData == null)
                throw new Exception($"{typeof(UserService)} exception, IsSuccessStatusCode=true, responseData=null");

            var imageUrl = responseData.Data[0];

            user.ProfileImage = imageUrl;
            _userRepository.Update(user);
            await _redisRepository.SetValueAsync($"user_{user.Id}",user);
            return Response<string>.Success($"Image changed with {imageUrl}",
                ResponseStatus.Success);
        }   
    }

    public async Task<Response<NoContent>> DeleteProfileImage(string userId)
    {
        _User user = await _userRepository.GetFirstAsync(u => u.Id == userId);
        if (user == null)
            return Response<NoContent>.Fail("User Not Found", ResponseStatus.NotFound);

        if (user.ProfileImage == null)
            return Response<NoContent>.Success(ResponseStatus.Success);
        NameObject nameObject = new() { Name = user.ProfileImage};
        var request = new RestRequest(ServiceConstants.API_GATEWAY + "/file/delete-user-image").AddBody(nameObject);
        var response = await _client.ExecutePostAsync<Response<string>>(request);

        if (!response.IsSuccessful || !response.Data!.IsSuccess)
            return Response<NoContent>.Fail("Failed", ResponseStatus.Failed);
                
        user.ProfileImage = null;
        _userRepository.Update(user);
        await _redisRepository.SetValueAsync($"user_{user.Id}",user);
        return Response<NoContent>.Success(ResponseStatus.Success);
    }

}