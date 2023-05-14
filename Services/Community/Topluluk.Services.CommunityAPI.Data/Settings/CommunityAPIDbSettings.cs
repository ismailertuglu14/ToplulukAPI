using System;
using DBHelper.Connection;
using Microsoft.Extensions.Configuration;

namespace Topluluk.Services.CommunityAPI.Data.Settings
{
	public class CommunityAPIDbSettings : IDbConfiguration
	{
		private readonly IConfiguration _configuration;

		public CommunityAPIDbSettings(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string ConnectionString { get { return _configuration.GetConnectionString("MongoDB");; } }
        public string DatabaseName { get { return "Community"; } }
    }
}

