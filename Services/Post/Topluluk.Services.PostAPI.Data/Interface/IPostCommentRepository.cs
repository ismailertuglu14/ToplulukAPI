using System;
using System.Linq.Expressions;
using DBHelper.Repository;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Interface
{
	public interface IPostCommentRepository : IGenericRepository<PostComment>
	{
		Task<List<PostComment>> GetPostCommentsDescendingDate(int skip, int take, Expression<Func<PostComment,bool>> predicate);
		Task<bool> DeletePostsComments(string userId);
		Task<Dictionary<string, int>> GetPostCommentCounts(List<string> postIds);
		Task<Dictionary<string, int>> GetCommentsReplyCounts(List<string> commentIds);
	}
}

