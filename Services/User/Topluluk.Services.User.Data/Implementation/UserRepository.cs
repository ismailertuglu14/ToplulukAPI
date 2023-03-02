using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using MongoDB.Driver;
using Topluluk.Services.User.Data.Interface;
using Topluluk.Shared.Dtos;
using _User = Topluluk.Services.User.Model.Entity.User;

namespace Topluluk.Services.User.Data.Implementation
{
	public class UserRepository : MongoGenericRepository<_User>, IUserRepository
	{
		public UserRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}
      

    }
}

