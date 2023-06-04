using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Dto.Http;
using Topluluk.Services.PostAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Controllers
{
    public class PostController : BaseController
    {
        

        private readonly IPostService _postService;
    
        private readonly ITestPostService _test;
        public PostController(IPostService postService, ITestPostService test)
        {
            _postService = postService;
            _test = test;
        }
    
        [HttpPost("test/create")]
        public async Task<Response<NoContent>> CreateTestPost(int count)
        {
            return await _test.CreatePostsForTest(count);
        }

        [HttpGet("feed")]
        public async Task<Response<List<GetPostForFeedDto>>> GetPostsForFeedScreen(int take = 10, int skip = 0)
        {
            return await _postService.GetPostForFeedScreen(this.UserId,this.Token,skip,take);
        }
        [HttpGet("community/{communityId}")]
        public async Task<Response<List<GetPostForFeedDto>>> GetCommunityPosts(string communityId, int take = 10, int skip = 0)
        {
            return await _postService.GetCommunityPosts(this.UserId, communityId,skip,take);
        }
        [HttpGet("user/{id}")]
        public async Task<Response<List<GetPostForFeedDto>>> GetUserPosts(string id, int take,int skip)
        {
            return await _postService.GetUserPosts(this.UserId,id,take,skip);
        }

        [HttpGet("GetPost")]
        public async Task<Response<GetPostByIdDto>> GetPostById(string id)
        {
            return await _postService.GetPostById(id, this.UserId);
        }

        [HttpPost("Create")]
        public async Task<Response<string>> Create( [FromForm] CreatePostDto postDto)
        {
            postDto.UserId = UserId;
            return await _postService.Create(this.UserId, postDto);
        }

        [HttpPost("Delete")]
        public async Task<Response<string>> Delete(PostDeleteDto postDto)
        {
            postDto.UserId = UserId;
            return await _postService.Delete(postDto);
        }


     

      

        [HttpGet("saved-posts")]
        public async Task<Response<List<GetPostForFeedDto>>> GetSavedPosts()
        {
            return await _postService.GetSavedPosts(this.UserId);
        }
        [HttpPost("save/{postId}")]
        public async Task<Response<string>> SavePost(string postId)
        {
            return await _postService.SavePost(this.UserId, postId);
        }
        [HttpGet("interactions/{postId}")]
        public async Task<Response<List<UserInfoDto>>> GetInteractions(string postId,int type, int take, int skip)
        {
            return await _postService.GetInteractions(this.UserId,postId,type, take,skip);
        }
        [HttpPost("interaction/{postId}")]
        public async Task<Response<string>> Interaction(string postId,PostInteractionCreateDto createDto)
        {
            return await _postService.Interaction(this.UserId,postId, createDto);
        }
        [HttpPost("remove-interaction/{postId}")]
        public async Task<Response<string>> RemoveInteraction(string postId)
        {
            return await _postService.RemoveInteraction(this.UserId, postId);
        }


        // Http Calls

        [HttpPost("delete-posts")]
        public async Task<Response<bool>> DeletePosts()
        {
            return await _postService.DeletePosts(this.UserId);
        }
    }
}

