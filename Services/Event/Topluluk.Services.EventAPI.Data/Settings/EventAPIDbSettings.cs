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

        public string ConnectionString => _configuration.GetConnectionString("MongoDB") ?? throw new ArgumentNullException();
        public string DatabaseName => "Event";
    }
}

