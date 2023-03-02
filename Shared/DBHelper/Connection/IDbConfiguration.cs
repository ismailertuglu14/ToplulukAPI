using System;
namespace DBHelper.Connection
{
    public interface IDbConfiguration
    {
        public string ConnectionString { get; }

        public string DatabaseName { get; }
    }
}

