using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using DBHelper.Repository.SQL;
using MongoDB.Driver;
using Topluluk.Services.AuthenticationAPI.Data.Interface;
using Topluluk.Services.AuthenticationAPI.Model.Entity;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.AuthenticationAPI.Data.Implementation
{
	public class AuthenticationRepository : MongoGenericRepository<UserCredential> , IAuthenticationRepository
	{
        private readonly IConnectionFactory _connectionFactory;

        public AuthenticationRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _connectionFactory = connectionFactory;
		}
        private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;

        public DatabaseResponse UpdateRefreshToken(UserCredential entity)
        {
            var database = GetConnection();
            var collectionName = string.Format("{0}Collection", typeof(UserCredential).Name);

            var data = database.GetCollection<UserCredential>(collectionName).ReplaceOne(x => x.Id == entity.Id, entity);
            DatabaseResponse response = new();
            response.Data = data;
            response.IsSuccess = true;

            return response;
        }

    }
}

