using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Implementation
{
	public class PostCommentRepository: MongoGenericRepository<PostComment>, IPostCommentRepository
	{
        private readonly IConnectionFactory _connectionFactory;

        public PostCommentRepository(IConnectionFactory connection) : base(connection)
		{
            _connectionFactory = connection;
        }
        private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;

        private string GetCollectionName() => string.Format("{0}Collection", typeof(PostComment).Name);

        public async Task<bool> DeletePostsComments(string userId)
        {
            try
            {
                var database = GetConnection();
                var collectionName = GetCollectionName();

                var filter = Builders<PostComment>.Filter.And(
                    Builders<PostComment>.Filter.Eq(p => p.UserId, userId),
                    Builders<PostComment>.Filter.Eq(p => p.IsDeleted, false));

                var update = Builders<PostComment>.Update.Set(p => p.IsDeleted, true);

                database.GetCollection<PostComment>(collectionName).UpdateMany(filter, update);
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }
    }
}

