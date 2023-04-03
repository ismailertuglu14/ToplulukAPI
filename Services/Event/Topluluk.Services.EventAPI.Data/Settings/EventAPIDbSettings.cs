using System;
using DBHelper.Connection;

namespace Topluluk.Services.EventAPI.Data.Settings
{
	public class EventAPIDbSettings : IDbConfiguration
    {
        public string ConnectionString { get { return "mongodb+srv://ismail:ismail@cluster0.psznbcu.mongodb.net/?retryWrites=true&w=majority"; } }
        public string DatabaseName { get { return "Event"; } }
    }
}

