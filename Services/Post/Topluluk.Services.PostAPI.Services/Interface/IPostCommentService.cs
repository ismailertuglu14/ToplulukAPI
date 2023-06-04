using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.PostAPI.Services.Interface;

public interface IPostCommentService
{
    Task<Response<List<CommentGetDto>>> GetComments(string userId, string postId, int take = 10, int skip = 0);
    Task<Response<NoContent>> CreateComment(CommentCreateDto commentDto);
    Task<Response<NoContent>> UpdateComment(string userId, CommentUpdateDto commentDto);
    Task<Response<NoContent>> DeleteComment(string userId, string commentId);


}