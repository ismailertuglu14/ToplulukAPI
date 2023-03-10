using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Implementation
{
	public class PostCommentRepository: MongoGenericRepository<PostComment>, IPostCommentRepository
	{
		public PostCommentRepository(IConnectionFactory connection) : base(connection)
		{
		}
	}
}

