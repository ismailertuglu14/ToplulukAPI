using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Driver;
using Topluluk.Services.User.Data.Interface;
using Topluluk.Services.User.Model.Dto;
using Topluluk.Services.User.Model.Entity;
using Topluluk.Shared.Dtos;
using _User = Topluluk.Services.User.Model.Entity.User;

namespace Topluluk.Services.User.Data.Implementation
{
	public class UserRepository : MongoGenericRepository<_User>, IUserRepository
	{
        private readonly IConnectionFactory _connectionFactory;
		public UserRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
            _connectionFactory = connectionFactory;
        }

        private IMongoDatabase GetConnection() => (MongoDB.Driver.IMongoDatabase)_connectionFactory.GetConnection;

        private string GetCollectionName() => string.Format("{0}Collection", typeof(_User).Name);

        public Task<bool> CheckIsUsernameUnique(string userName)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            return database.GetCollection<_User>(collectionName).Find(u => u.UserName == userName).AnyAsync();
         
        }
        public Task<bool> CheckIsEmailUnique(string email)
        {
            var database = GetConnection();
            var collectionName = GetCollectionName();

            return database.GetCollection<_User>(collectionName).Find(u => u.Email == email).AnyAsync();
        }

        
    }
}

