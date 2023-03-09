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
        private readonly IMapper _mapper;
        private readonly RestClient _client;

        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _client = new RestClient();
        }

        public async Task<Response<string>> Comment(CommentCreateDto commentDto)
        {
            Comment comment = _mapper.Map<Comment>(commentDto);

            Post? post = await _postRepository.GetFirstAsync(p => p.Id == commentDto.PostId);

            if (post != null)
            {
                post.Comments.Add(comment);
                _postRepository.Update(post);

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
            //Servislerden birisi false dönerse post silinmesini iptal etmek adına yeniden postu yazıcaz.
            bool cSuccess = false;
            bool uSuccess = false;

            Post post = await _postRepository.GetFirstAsync(p => p.Id == postDto.PostId);

            _postRepository.Delete(post.Id);

            // Topluluğa aitse topluluktan kaldır
            if (post.CommunityId != null)
            {
                // Community servisine post silinme isteği
            //    var communityDeleteRequest = new RestRequest("https://").AddBody();
              //  var communityDeleteResponse = await _client.ExecutePostAsync(communityDeleteRequest);
            }

            // User servisine post silinme isteği

            PostDeleteDto postDeleteDto = new() { PostId = postDto.PostId, UserId = postDto.UserId };
            var userDeleteRequest = new RestRequest("https://localhost:7202/User/DeletePost").AddBody(postDeleteDto);
            var userDeleteResponse = await _client.ExecutePostAsync(userDeleteRequest);

            if (userDeleteResponse.IsSuccessStatusCode == true)
            {
                return await Task.FromResult(Response<string>.Success("Success", Shared.Enums.ResponseStatus.Success));
            }

            return await Task.FromResult(Response<string>.Fail("Failed", Shared.Enums.ResponseStatus.InitialError));
        }

        public Task<Response<string>> DeleteComment(string userId, string commentId)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> GetCommunityPosts(string communityId, int skip = 0, int take = 10)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> GetPostById(string postId, bool isDeleted = false)
        {
            throw new NotImplementedException();
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

