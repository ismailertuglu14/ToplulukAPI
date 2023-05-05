using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PostController : BaseController
    {
        

        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }


        [HttpGet("feed")]
        public async Task<Response<List<GetPostForFeedDto>>> GetPostsForFeedScreen(int take = 10, int skip = 0)
        {
            return await _postService.GetPostForFeedScreen(this.UserId,this.Token,skip,take);
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

        [HttpPost("[action]")]
        public async Task<Response<string>> Create( [FromForm] CreatePostDto postDto)
        {
            postDto.UserId = UserId;
            return await _postService.Create(this.UserId, postDto);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> Delete(PostDeleteDto postDto)
        {
            postDto.UserId = UserId;
            return await _postService.Delete(postDto);
        }


        [HttpPost("[action]")]
        public async Task<Response<string>> Comment(CommentCreateDto commentDto)
        {
            commentDto.UserId = UserId;
            return await _postService.Comment(commentDto);
        }

        [HttpGet("comments")]
        public async Task<Response<List<CommentGetDto>>> GetComments(string id)
        {
            return await _postService.GetComments(this.UserId, id);
        }

        [HttpPost("comment/delete/{id}")]
        public async Task<Response<string>> DeleteComment(string id)
        {
            return await _postService.DeleteComment(this.UserId, id);
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
        public async Task<Response<List<GetPostInteractionDto>>> GetInteractions(string postId, int take, int skip)
        {
            return await _postService.GetInteractions(this.UserId,postId,take,skip);
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

