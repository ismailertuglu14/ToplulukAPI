using System;
using DBHelper.Connection;

namespace Topluluk.Services.FileAPI.Data.Settings
{
	public class FileAPIDbSettings : IDbConfiguration
	{
        public string ConnectionString { get { return ""; } }

        public string DatabaseName { get { return ""; } }
    }
}

