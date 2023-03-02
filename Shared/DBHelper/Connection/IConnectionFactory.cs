using System;
namespace DBHelper.Connection
{
    public interface IConnectionFactory
    {
        dynamic GetConnection { get; }
    }
}

