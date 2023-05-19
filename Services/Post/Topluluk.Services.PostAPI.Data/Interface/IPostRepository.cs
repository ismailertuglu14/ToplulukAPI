using System;
using System.Linq.Expressions;
using DBHelper.Repository;
using DBHelper.Repository.Mongo;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Interface
{
	public interface IPostRepository : IGenericRepository<Post>
	{
		Task<bool> DeletePosts(string userId);
		Task<List<Post>> GetPostsWithDescending(int skip, int take, Expression<Func<Post, bool>> expression);

	}
}

