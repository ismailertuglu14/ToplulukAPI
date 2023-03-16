using System;
using AutoMapper;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Services.PostAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using System.Net.Http;
using Topluluk.Shared.Helper;
using Topluluk.Shared.Enums;
using MongoDB.Bson.IO;
using System.Text;
using System.Text.Json;
using DotNetCore.CAP;
using Topluluk.Services.PostAPI.Model.Dto.Http;
using RestSharp;

namespace Topluluk.Services.PostAPI.Services.Implementation
{
	public class PostService : IPostService
	{

        private readonly IPostRepository _postRepository;
        private readonly IPostCommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly RestClient _client;

        public PostService(IPostRepository postRepository, IPostCommentRepository commentRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _commentRepository = commentRepository;
            _client = new RestClient();
        }

        public async Task<Response<string>> Comment(CommentCreateDto commentDto)
        {
            PostComment comment = _mapper.Map<PostComment>(commentDto);

            Post? post = await _postRepository.GetFirstAsync(p => p.Id == commentDto.PostId);

            if (post != null)
            {
                await _commentRepository.InsertAsync(comment);
                return await Task.FromResult(Response<string>.Success("Success", Shared.Enums.ResponseStatus.Success));
            }

            return await Task.FromResult(Response<string>.Fail("Post not found", Shared.Enums.ResponseStatus.NotFound));
        }

        public async Task<Response<string>> Create(CreatePostDto postDto)
        {
            Post post = _mapper.Map<Post>(postDto);

            DatabaseResponse response = await _postRepository.InsertAsync(post);

            // Post topluluk da paylaşılacak
            if (post.CommunityId != null)
            {
                // todo RestRequest rewrite
                var _community = HttpRequestHelper.handle<string>(post.CommunityId, $"https://localhost:7132/Community/Participiants/{post.CommunityId}", HttpType.GET).Result;
                var _participiants = _community.Content.ReadAsStringAsync().Result;
                List<string>? participiants = JsonSerializer.Deserialize<List<string>>(_participiants);


                // Kullanıcı topluluk içinde değilse paylaşamasın.
                if (!participiants!.Contains(postDto.UserId))
                {

                    // Post silindi ve fonksiyon bitirildi.
                    _postRepository.Delete(post);
                    return await Task.FromResult(Response<string>.Fail("Failed", Shared.Enums.ResponseStatus.NotAuthenticated));
                }
                // Toplulukda da bu post paylaşılabilsin.
                else
                {
                    PostCreatedCommunityDto body = new() { Id = response.Data, CommunityId = postDto.CommunityId };
                    var communityCreateRequest = new RestRequest("https://localhost:7132/community/postcreated").AddBody(body);
                    var communityCreateResponse = await _client.ExecutePostAsync(communityCreateRequest);
                    

                    if (communityCreateResponse.IsSuccessStatusCode == false)
                    {

                        // Post silindi ve fonksiyon bitirildi.
                        _postRepository.Delete(post);
                        return await Task.FromResult(Response<string>.Fail("Failed", Shared.Enums.ResponseStatus.NotAuthenticated));
                    }


                }

            }

            PostCreatedUserDto postCreatedUserDto = new() { PostId = response.Data, UserId = postDto.UserId};
            var userCreateRequest = new RestRequest("https://localhost:7202/User/PostCreated").AddBody(postCreatedUserDto);
            var userCreateResponse = await _client.ExecutePostAsync(userCreateRequest);

            if (userCreateResponse.IsSuccessStatusCode == true)
            {
                return await Task.FromResult(Response<string>.Success(response.Data, Shared.Enums.ResponseStatus.Success));
            }
            else
            {
                // Post silindi ve fonksiyon bitirildi.
                _postRepository.Delete(post);
                return await Task.FromResult(Response<string>.Fail("Failed",Shared.Enums.ResponseStatus.InitialError));
            }
        }

        public async Task<Response<string>> Delete(PostDeleteDto postDto)
        {

            Post post = await _postRepository.GetFirstAsync(p => p.Id == postDto.PostId);

            _postRepository.DeleteById(post.Id);

            return await Task.FromResult(Response<string>.Success("Success", Shared.Enums.ResponseStatus.Success));
        }

        public Task<Response<string>> DeleteComment(string userId, string commentId)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> GetCommunityPosts(string communityId, int skip = 0, int take = 10)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<GetPostByIdDto>> GetPostById(string postId, string sourceUserId, bool isDeleted = false)
        {
            GetPostByIdDto postDto = new();

            Post post = await _postRepository.GetFirstAsync(p => p.Id == postId);

            postDto.Id = postId;
            postDto.Description = post.Description;
            postDto.CreatedAt = post.CreatedAt ?? DateTime.Now;
            postDto.Files = post.Files;
            postDto.InteractionCount = post.Interactions.Count;

           var _comments = await _commentRepository.GetAllAsync(10, 0, c => c.PostId == postId);
            if (_comments.Data != null && _comments.Data.Count > 0)
            {
                postDto.CommentCount = _comments.Data?.Count;

                CommentGetDto commentDto = new();

                foreach (PostComment c in _comments.Data)
                {
                    commentDto.Id = c.Id;
                    //commentDto.
                }


                postDto.Comments = _comments.Data;
            }


            if (post.CommunityId != null)
            {
                // Get community title request
                var communityGetTitleRequest = new RestRequest("https://localhost:7132/Community/getCommunityTitle").AddParameter("id", post.CommunityId);
                var communityGetTitleResponse = await _client.ExecuteGetAsync<Response<string>>(communityGetTitleRequest);
                var communityResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Response<string>>(communityGetTitleResponse.Content);
                postDto.CommunityTitle = communityResponse?.Data;
            }

            var userInfoRequest = new RestRequest("https://localhost:7202/User/GetUserInfoForPost")
                .AddParameter("id", post.UserId)
                .AddParameter("sourceUserId", sourceUserId);
            var userInfoResponse = await _client.ExecuteGetAsync<Response<UserInfoGetResponse>>(userInfoRequest);
            var userResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Response<UserInfoGetResponse>>(userInfoResponse.Content);

            postDto.UserId = userResponse.Data.UserId;
            postDto.FirstName = userResponse.Data.FirstName;
            postDto.LastName = userResponse.Data.LastName;
            postDto.IsUserFollowing = userResponse.Data.IsUserFollowing;
            postDto.ProfileImage = userResponse.Data.ProfileImage;
            postDto.UserName = userResponse.Data.UserName;


            

            return await Task.FromResult(Response<GetPostByIdDto>.Success(postDto, Shared.Enums.ResponseStatus.Success));
        }

        // Feed ekranındaki postlar için hazırlanmış metod.
        public async Task<Response<List<GetPostDto>>> GetPosts(string userId, int take = 10, int skip = 0)
        {
            // User servise istek atıp takip ettiğimiz kullanıcıların id listesini döndürcez.

            // Kullanıcının takip ettiği kullanıcıların postları, kendi postları, kendi katıldığı topluluklarının postları
            var response = await _postRepository.GetAllAsync(take, skip, p => p.UserId == userId);

            return await Task.FromResult(Response<List<GetPostDto>>.Success(response.Data, Shared.Enums.ResponseStatus.Success));

        }

        public Task<Response<string>> Interaction(string postId, InteractionType interactionType)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> Update()
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> UpdateComment(string userId, string commentId, string newComment)
        {
            throw new NotImplementedException();
        }

    }
}

