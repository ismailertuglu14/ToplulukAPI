using System;
using DBHelper.Repository;
using DBHelper.Repository.Mongo;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Interface
{
	public interface IPostRepository : IGenericRepository<Post>
	{
	}
}

