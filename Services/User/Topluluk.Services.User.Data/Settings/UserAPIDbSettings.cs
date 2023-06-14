using System;
using DBHelper.Connection;
using Microsoft.Extensions.Configuration;

namespace Topluluk.Services.User.Data.Settings
{
	public class UserAPIDbSettings : IDbConfiguration
	{
		private readonly IConfiguration _configuration;
    
		public UserAPIDbSettings(IConfiguration configuration)
		{
			_configuration = configuration;
		}
        public string ConnectionString => _configuration.GetConnectionString("MongoDB") ?? throw new Exception("Use API Db Settings con string null"); 
        public string DatabaseName { get { return "User"; } }
    }
}

