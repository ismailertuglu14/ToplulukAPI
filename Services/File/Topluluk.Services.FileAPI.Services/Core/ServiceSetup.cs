using System;
using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Topluluk.Services.FileAPI.Data.Settings;
using Topluluk.Services.FileAPI.Services.Implementation;
using Topluluk.Services.FileAPI.Services.Interface;

namespace Topluluk.Services.FileAPI.Services.Core
{
    public static class ServiceSetup
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            AddServicesForRepository(services);
        }

        public static void AddServicesForRepository(this IServiceCollection services)
        {
            services.AddSingleton<IDbConfiguration, FileAPIDbSettings>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
            services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>();
        }

        public static void AddStorage<T>(this IServiceCollection services) where T : Storage, IStorage
        {
           services.AddScoped<IStorage, T>();
        }

    }
}

