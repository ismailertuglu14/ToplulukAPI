using System;
using DBHelper.BaseDto;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace DBHelper.Connection.Mongo
{
    public class MongoConnectionFactory : IConnectionFactory
    {
        private readonly IBaseDatabaseSettings _configuration;
        public MongoConnectionFactory(IBaseDatabaseSettings configuration)
        {
            _configuration = configuration;
            if (_configuration.DBType == DatabaseType.MongoDB)
            {
                var client = new MongoClient(configuration.ConnectionString);
                GetConnection = client.GetDatabase(configuration.DatabaseName);

                var ignoreExtraElementsConvention = new ConventionPack { new IgnoreExtraElementsConvention(true) };
                ConventionRegistry.Register("IgnoreExtraElements", ignoreExtraElementsConvention, type => true);
            }
        }

        public object GetConnection
        {
            get; set;
        }

    }
}

