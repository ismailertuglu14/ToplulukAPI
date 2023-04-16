using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.EventAPI.Services.Interface;

public interface IEventCommentService
{
    Task<Response<string>> CreateComment(string userId, CommentCreateDto dto);
    Task<Response<NoContent>> DeleteComment(string id, string eventId);
    Task<Response<List<GetEventCommentDto>>> GetEventComments(string userId, string id, int skip = 0, int take = 10);

}