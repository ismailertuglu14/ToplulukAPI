using System;
using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Topluluk.Services.QRAPI.Services.Implementation;
using Topluluk.Services.QRAPI.Services.Interface;

namespace Topluluk.Services.QRAPI.Services.Core
{
    public static class ServiceSetup
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            AddServicesForRepository(services);
            AddServicesForServices(services);
            //AddServicesForLangServices(services);
        }

        public static void AddServicesForRepository(this IServiceCollection services)
        {
            //services.AddSingleton<IDbConfiguration, AuthenticationAPIDbSettings>();
            //services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
            //services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>();
            // services.AddTransient<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            // services.AddTransient<IErrorRepository, ErrorRepository>();
            // services.AddTransient<IRequestResponseLogRepository, RequestResponseLogRepository>();
        }

        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddTransient<IQRService, QRService>();
        }

      
    }
}

