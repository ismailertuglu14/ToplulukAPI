using System;
namespace DBHelper.BaseDto
{
    public interface IBaseDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public DatabaseType DBType { get; set; }
    }
}

