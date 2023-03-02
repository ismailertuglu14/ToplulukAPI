using System;
using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Topluluk.Services.User.Data.Implementation;
using Topluluk.Services.User.Data.Interface;
using Topluluk.Services.User.Data.Settings;
using Topluluk.Services.User.Services.Implementation;
using Topluluk.Services.User.Services.Interface;

namespace Topluluk.Services.User.Services.Core
{
    public static class ServiceSetup
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            AddServicesForRepository(services);
            AddServicesForServices(services);
        }

        public static void AddServicesForRepository(this IServiceCollection services)
        {
            services.AddSingleton<IDbConfiguration, UserAPIDbSettings>();
            services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
            services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>();
            // services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            // services.AddTransient<IErrorRepository, ErrorRepository>();
            // services.AddTransient<IRequestResponseLogRepository, RequestResponseLogRepository>();
        }

        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
        }


    }
}

