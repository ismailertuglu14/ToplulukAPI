using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
            return await _postService.GetPostForFeedScreen(this.UserId,skip,take);
        }

        [HttpGet("user")]
        public async Task<Response<List<GetPostDto>>> GetPostsForUserScreen(int take,int skip)
        {
            return await _postService.GetUserPosts(this.UserId,take,skip);
        }

        [HttpGet("GetPost")]
        public async Task<Response<GetPostByIdDto>> GetPostById(string id)
        {
            return await _postService.GetPostById(id, this.UserId);
        }

        [HttpPost("[action]")]
        public async Task<Response<string>> Create(CreatePostDto postDto)
        {
            postDto.UserId = UserId;
            return await _postService.Create(postDto);
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

        [HttpPost("save/{postId}")]
        public async Task<Response<string>> SavePost(string postId)
        {
            return await _postService.SavePost(this.UserId, postId);
        }

    }
}

