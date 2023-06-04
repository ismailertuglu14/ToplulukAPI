using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Controllers;

[ApiController]
[Route("Post")]
public class PostCommentController : BaseController
{
    private readonly IPostCommentService _commentService;
    public PostCommentController(IPostCommentService commentService)
    {
        _commentService = commentService;
    }
    
    [HttpGet("comments")]
    public async Task<Response<List<CommentGetDto>>> GetComments(string id)
    {
        return await _commentService.GetComments(this.UserId, id);
    }
    
    
    [HttpPost("Comment")]
    public async Task<Response<NoContent>> Comment(CommentCreateDto commentDto)
    {
        commentDto.UserId = UserId;
        return await _commentService.CreateComment(commentDto);
    }
    
    
    [HttpPost("comment/delete/{id}")]
    public async Task<Response<NoContent>> DeleteComment(string id)
    {
        return await _commentService.DeleteComment(this.UserId, id);
    }
   
}