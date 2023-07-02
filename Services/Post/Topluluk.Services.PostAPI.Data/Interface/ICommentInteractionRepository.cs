using DBHelper.Repository;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Interface;

public interface ICommentInteractionRepository: IGenericRepository<CommentInteraction>
{
    Task<Dictionary<string, CommentLikes>> GetCommentsInteractionCounts(List<string> commentIds);
    Task<Dictionary<string, CommentInteracted>> GetCommentsIsInteracted(string userId, List<string> commentIds);
}