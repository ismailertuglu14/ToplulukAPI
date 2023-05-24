using DBHelper.Repository;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Interface;

public interface ISavedPostRepository : IGenericRepository<SavedPost>
{
    Task<bool> DeleteSavedPostsByUserId(string userId);
    Task<Dictionary<string, bool>> IsUserSavedPosts(string userId, List<string> postIds);
}