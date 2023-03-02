using System;
using DBHelper.BaseDto;

namespace DBHelper.Connection.SQL
{
    public class SqlDatabaseSettings : IBaseDatabaseSettings
    {
        private readonly IDbConfiguration _dbConfiguration;
        public SqlDatabaseSettings(IDbConfiguration dbConfiguration)
        {
            _dbConfiguration = dbConfiguration;

            this.ConnectionString = _dbConfiguration.ConnectionString;
            this.DatabaseName = _dbConfiguration.DatabaseName;
            this.DBType = DatabaseType.MSSQL;
        }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public DatabaseType DBType { get; set; }
    }
}

