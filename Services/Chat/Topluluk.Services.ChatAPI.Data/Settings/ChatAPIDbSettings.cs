﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBHelper.Connection;
using Microsoft.Extensions.Configuration;

namespace Topluluk.Services.ChatAPI.Data.Settings
{
    public class ChatAPIDbSettings : IDbConfiguration
    {
        private readonly IConfiguration _configuration;

        public ChatAPIDbSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString { get { return _configuration.GetConnectionString("MongoDB");; } }
        public string DatabaseName { get { return "Topluluk"; } }
    }
}
