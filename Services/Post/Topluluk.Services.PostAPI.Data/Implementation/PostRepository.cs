using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Model.Entity;

namespace Topluluk.Services.PostAPI.Data.Implementation
{
	public class PostRepository : MongoGenericRepository<Post>,  IPostRepository
	{

        private readonly IConnectionFactory _connectionFactory;

		public PostRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
            _connectionFactory = connectionFactory;
        }
        private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;

        private string GetCollectionName() => string.Format("{0}Collection", typeof(Post).Name);

        public Task<bool> DeletePosts(string userId)
        {
            try
            {
                var database = GetConnection();
                var collectionName = GetCollectionName();

                var filter = Builders<Post>.Filter.And(
                    Builders<Post>.Filter.Eq(p => p.UserId, userId),
                    Builders<Post>.Filter.Eq(p => p.IsDeleted, false));

                var update = Builders<Post>.Update.Set(p => p.IsDeleted, true);

                database.GetCollection<Post>(collectionName).UpdateMany(filter, update);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
         }
    }
}

