using System;
using DBHelper.BaseDto;

namespace DBHelper.Connection.Mongo
{
    public class MongoDatabaseSettings : IBaseDatabaseSettings
    {
        private readonly IDbConfiguration _dbConfiguration;
        public MongoDatabaseSettings(IDbConfiguration dbConfiguration)
        {
            _dbConfiguration = dbConfiguration;

            this.ConnectionString = _dbConfiguration.ConnectionString;
            this.DatabaseName = _dbConfiguration.DatabaseName;
            this.DBType = DatabaseType.MongoDB;
        }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public DatabaseType DBType { get; set; }
    }
}

