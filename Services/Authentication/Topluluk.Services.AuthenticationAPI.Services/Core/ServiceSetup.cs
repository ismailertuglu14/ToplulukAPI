using System;
using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Topluluk.Services.AuthenticationAPI.Data.Implementation;
using Topluluk.Services.AuthenticationAPI.Data.Interface;
using Topluluk.Services.AuthenticationAPI.Data.Settings;
using Topluluk.Services.AuthenticationAPI.Services.Implementation;
using Topluluk.Services.AuthenticationAPI.Services.Interface;

namespace Topluluk.Services.AuthenticationAPI.Services.Core
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
            services.AddSingleton<IDbConfiguration, AuthenticationAPIDbSettings>();
            services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
            services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>(); 
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<ILoginLogRepository, LoginLogRepository>();
        }

        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }

   
    }
}

