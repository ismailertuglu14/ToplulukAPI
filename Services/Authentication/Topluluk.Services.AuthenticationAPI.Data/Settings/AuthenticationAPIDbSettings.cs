using System;
using DBHelper.Connection;

namespace Topluluk.Services.AuthenticationAPI.Data.Settings
{
    public class AuthenticationAPIDbSettings : IDbConfiguration
    {
        //public string ConnectionString { get { return "Server=localhost;Database=Topluluk;User Id=SA;Password=ismail123A+"; } }

        public string ConnectionString { get { return "mongodb+srv://ismail:ismail@cluster0.psznbcu.mongodb.net/?retryWrites=true&w=majority"; } }
        public string DatabaseName { get { return "Topluluk"; } }
    }
}

