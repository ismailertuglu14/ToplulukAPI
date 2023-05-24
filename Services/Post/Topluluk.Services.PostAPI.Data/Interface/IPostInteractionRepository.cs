using DBHelper.Repository;
using Topluluk.Services.PostAPI.Model.Dto;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Interface;

public interface IPostInteractionRepository : IGenericRepository<PostInteraction>
{		
    Task<Dictionary<string, PostInteraction>> IsUserInteractedPosts(string userId, List<string> postdIds);
    Task<Dictionary<string, int>> PostsInteractionCounts(List<string> postIds);
    Task<Dictionary<string, PostInteractionPreviewDto[]>> GetPostInteractionPreviews(List<string> postIds);
}