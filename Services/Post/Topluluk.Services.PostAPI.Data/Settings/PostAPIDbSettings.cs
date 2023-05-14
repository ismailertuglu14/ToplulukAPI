using System;
using DBHelper.Connection;
using Microsoft.Extensions.Configuration;

namespace Topluluk.Services.PostAPI.Data.Settings
{
	public class PostAPIDbSettings : IDbConfiguration
    {
        private readonly IConfiguration _configuration;

        public PostAPIDbSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString { get { return _configuration.GetConnectionString("MongoDB");; } }
        public string DatabaseName { get { return "Post"; } }
    }
}


