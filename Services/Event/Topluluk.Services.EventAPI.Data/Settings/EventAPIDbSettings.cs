using System;
using DBHelper.Connection;
using Microsoft.Extensions.Configuration;

namespace Topluluk.Services.EventAPI.Data.Settings
{
	public class EventAPIDbSettings : IDbConfiguration
    {
        private readonly IConfiguration _configuration;

        public EventAPIDbSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString { get { return _configuration.GetConnectionString("MongoDB");; } }
        public string DatabaseName { get { return "Event"; } }
    }
}

