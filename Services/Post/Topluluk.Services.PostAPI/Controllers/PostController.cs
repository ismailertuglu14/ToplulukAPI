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
    }
}

