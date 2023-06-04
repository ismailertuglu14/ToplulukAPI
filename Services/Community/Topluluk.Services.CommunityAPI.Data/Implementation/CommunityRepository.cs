using System;
using System.Linq.Expressions;
using System.Net;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;
using Topluluk.Services.CommunityAPI.Data.Interface;
using Topluluk.Services.CommunityAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.CommunityAPI.Data.Implementation
{
	public class CommunityRepository : MongoGenericRepository<Community>, ICommunityRepository
	{
        private readonly DBHelper.Connection.IConnectionFactory _connectionFactory;

        public CommunityRepository(DBHelper.Connection.IConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _connectionFactory = connectionFactory;
		}
        private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;

        private string GetCollectionName() => $"{nameof(Community)}Collection";

        public async Task<DatabaseResponse> AddItemToArrayProperty(string id, string arrayName, object item)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            var filter = Builders<Community>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<Community>.Update.Push(arrayName, item);
            var options = new FindOneAndUpdateOptions<Community> { ReturnDocument = ReturnDocument.After };

            await database.GetCollection<Community>(collectionName).FindOneAndUpdateAsync(filter, update, options);

            DatabaseResponse dbResponse = new();
            dbResponse.IsSuccess = true;
            dbResponse.Data = "Datas updated successfully";

            return dbResponse;
        }

        public Task<DatabaseResponse> RemoveItemFromArrayProperty<T>(string id, string arrayName, string addId)
        {
            throw new NotImplementedException();
        }

        public async Task<Community> GetFirstCommunity(Expression<Func<Community, bool>> predicate)
        {

            // GetFirstAsync(c => c.Id == communityId && c.IsVisible == true && c.IsRestricted == false);
            var database = GetConnection();
            var collectionName = GetCollectionName();

            var defaultFilter = Builders<Community>.Filter.Eq(c => c.IsDeleted, false);
            var finalFilter = Builders<Community>.Filter.And(defaultFilter, predicate);

            return await database!.GetCollection<Community>(collectionName).Find(finalFilter).FirstOrDefaultAsync();
        }
    }
}

