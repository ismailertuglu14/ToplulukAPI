using System;
namespace DBHelper.Connection.Mongo
{
    public class MongoDbConfiguration : IDbConfiguration
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}

