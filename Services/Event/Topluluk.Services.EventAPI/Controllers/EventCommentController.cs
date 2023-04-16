using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Services.EventAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.EventAPI.Controllers;

[Route("event")]
public class EventCommentController : BaseController
{

    private readonly IEventCommentService _commentService;

    public EventCommentController(IEventCommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost("create-comment")]
    [Authorize]
    public async Task<Response<string>> CreateComment(CommentCreateDto dto)
    {
        return await _commentService.CreateComment(this.UserId, dto);
    }
    [HttpPost("comment/delete/{id}")]
    [Authorize]
    public async Task<Response<NoContent>> DeleteComment(string id)
    {
        return await _commentService.DeleteComment(this.UserId, id);
    }
    [HttpGet("comments/{id}")]
    [Authorize]
    public async Task<Response<List<GetEventCommentDto>>> GetComments(string id, int take = 10, int skip = 0)
    {
        return await _commentService.GetEventComments(this.UserId, id, skip, take);
    }
}