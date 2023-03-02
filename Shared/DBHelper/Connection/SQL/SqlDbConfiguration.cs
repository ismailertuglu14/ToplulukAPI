using System;
namespace DBHelper.Connection.SQL
{
    public class SqlDbConfiguration : IDbConfiguration
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}

