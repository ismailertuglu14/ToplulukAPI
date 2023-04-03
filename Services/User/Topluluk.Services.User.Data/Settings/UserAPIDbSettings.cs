using System;
using DBHelper.Connection;
using Microsoft.Extensions.Configuration;

namespace Topluluk.Services.User.Data.Settings
{
	public class UserAPIDbSettings : IDbConfiguration
	{
        public string ConnectionString { get { return "mongodb+srv://ismail:ismail@cluster0.psznbcu.mongodb.net/?retryWrites=true&w=majority"; } }
        public string DatabaseName { get { return "User"; } }
    }
}

