using System;
using DBHelper.BaseDto;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace DBHelper.Connection.SQL
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        private readonly IBaseDatabaseSettings _configurationBase;
        private readonly IConfiguration _configuration;
        private string _connectionString;
        private IDbConnection _connection;
        public SqlConnectionFactory(IBaseDatabaseSettings baseConfiguration, IConfiguration configuration)
        {
            _configurationBase = baseConfiguration;
            _configuration = configuration;

            if (baseConfiguration.DBType == DatabaseType.MSSQL)
            {
                _configuration = configuration;
                _connectionString = _configurationBase.ConnectionString;
            }

        }
        public object GetConnection
        {
            get; set;
        }
    }
}

