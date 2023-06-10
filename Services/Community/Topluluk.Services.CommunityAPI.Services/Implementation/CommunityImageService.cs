using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using Topluluk.Services.CommunityAPI.Data.Interface;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Services.CommunityAPI.Services.Interface;
using Topluluk.Services.FileAPI.Model.Dto.Http;
using Topluluk.Shared.Constants;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Exceptions;
using ResponseStatus = Topluluk.Shared.Enums.ResponseStatus;

namespace Topluluk.Services.CommunityAPI.Services.Implementation;

public class CommunityImageService : ICommunityImageService
{
    private readonly RestClient _client;
    private readonly ICommunityRepository _communityRepository;


    public CommunityImageService(ICommunityRepository communityRepository)
    {
        _communityRepository = communityRepository;
        _client = new RestClient();
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
        }
    
    public async Task<Response<string>> UpdateBannerImage(string userId, string communityId, BannerImageUpdateDto dto)
        {
            
            Community community = await _communityRepository.GetFirstAsync(c => c.Id == communityId);

            if (community == null)
                throw new NotFoundException("Community Not Found");

            if (community.AdminId != userId)
                throw new UnauthorizedAccessException();

            using (var client = new HttpClient())
            {
                
                byte[] imageBytes;

                using (var stream = new MemoryStream())
                {
                    await dto.File.CopyToAsync(stream);
                    imageBytes = stream.ToArray();
                }

                var content = new MultipartFormDataContent();
                Response<string>? responseData = new();

                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                content.Add(imageContent, "File", dto.File.FileName);


                if (!community.CoverImage.IsNullOrEmpty())
                {

                    NameObject nameObject = new() { Name = community.CoverImage! };
                    var deleteBannerImageRequest = new RestRequest(ServiceConstants.API_GATEWAY + "/file/delete-community-banner-image").AddBody(nameObject);
                    var deleteCoverImageResponse = await _client.ExecutePostAsync<Response<string>>(deleteBannerImageRequest);
                }
                var response = await client.PostAsync("https://localhost:7165/file/upload-community-banner-image", content);
                if (response.IsSuccessStatusCode)
                {
                    responseData = await response.Content.ReadFromJsonAsync<Response<string>>(); 
                    if (responseData != null)
                    {
                        var imageUrl = responseData.Data;

                        community.BannerImage = imageUrl;
                        _communityRepository.Update(community);
                        return Response<string>.Success(imageUrl, ResponseStatus.Success);
                    }

                    throw new Exception($"{typeof(CommunityService)} exception, IsSuccessStatusCode=true, responseData=null");
                }
                else
                {
                    // Resim yükleme işlemi başarısız
                    return Response<string>.Fail("Failed while uploading image with http client", ResponseStatus.InitialError);
                }

            }
        }

    public async Task<Response<NoContent>> RemoveCoverImage(string userId, string communityId)
    {
         
        Community community = await _communityRepository.GetFirstAsync(c => c.Id == communityId);

        if (community == null)
            throw new NotFoundException("Community Not Found");

        if (community.AdminId != userId)
            throw new UnauthorizedAccessException();
        
        community.CoverImage = null;
        _communityRepository.Update(community);
        return Response<NoContent>.Success(ResponseStatus.Success);
    }

    public async Task<Response<NoContent>> RemoveBannerImage(string userId, string communityId)
    {
         
        Community community = await _communityRepository.GetFirstAsync(c => c.Id == communityId);

        if (community == null)
            throw new NotFoundException("Community Not Found");

        if (community.AdminId != userId)
            throw new UnauthorizedAccessException();
        
        community.BannerImage = null;
        _communityRepository.Update(community);
        return Response<NoContent>.Success(ResponseStatus.Success);
    }
}