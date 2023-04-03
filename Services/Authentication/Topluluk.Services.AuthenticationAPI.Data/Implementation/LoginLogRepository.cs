using System;
using DBHelper.Connection;
using DBHelper.Repository.Mongo;
using Topluluk.Services.AuthenticationAPI.Data.Interface;
using Topluluk.Services.AuthenticationAPI.Model.Entity;

namespace Topluluk.Services.AuthenticationAPI.Data.Implementation
{
	public class LoginLogRepository : MongoGenericRepository<LoginLog>, ILoginLogRepository
    {
		public LoginLogRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
		}
	}
}

