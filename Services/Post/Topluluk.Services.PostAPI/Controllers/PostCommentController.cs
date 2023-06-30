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
    
    [HttpGet("{id}/comments")]
    public async Task<Response<List<CommentGetDto>>> GetComments(string id, int skip, int take)
    {
        return await _commentService.GetComments(this.UserId, id, skip, take);
    }
    
    [HttpGet("comment/{commentId}/replies")]
    public async Task<Response<List<CommentGetDto>>> GetReplies(string commentId, int skip, int take)
    {
        return await _commentService.GetReplies(commentId, skip, take);
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
    
    [HttpPut("comment/{id}/update")]
    public async Task<Response<NoContent>> UpdateComment(string id, CommentUpdateDto dto)
    {
        dto.CommentId = id;
        return await _commentService.UpdateComment(this.UserId, dto);
    }

    [HttpPost("comment/{commentId}/interaction")]
    public async Task<Response<NoContent>> Interaction(string commentId, int type)
    {
        return await _commentService.Interaction(this.UserId, commentId, type);
    }

    
}