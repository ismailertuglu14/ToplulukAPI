using System;
using DBHelper.Connection;
using Microsoft.Extensions.Configuration;

namespace Topluluk.Services.AuthenticationAPI.Data.Settings
{
    public class AuthenticationAPIDbSettings : IDbConfiguration
    {
        //public string ConnectionString { get { return "Server=localhost;Database=Topluluk;User Id=SA;Password=ismail123A+"; } }
        private readonly IConfiguration _configuration;
        public AuthenticationAPIDbSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string ConnectionString { get { return _configuration.GetConnectionString("MongoDB");; } }
        public string DatabaseName { get { return "User"; } }
    }
}

