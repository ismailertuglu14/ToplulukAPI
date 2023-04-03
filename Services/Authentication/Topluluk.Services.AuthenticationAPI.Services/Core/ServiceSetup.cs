using System;
using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using DBHelper.Connection.SQL;
using Microsoft.Extensions.DependencyInjection;
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
            AddServicesForLangServices(services);
        }

        public static void AddServicesForRepository(this IServiceCollection services)
        {
            services.AddSingleton<IDbConfiguration, AuthenticationAPIDbSettings>();
            services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
            services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>(); 
            // services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<ILoginLogRepository, LoginLogRepository>();
            // services.AddTransient<IErrorRepository, ErrorRepository>();
            // services.AddTransient<IRequestResponseLogRepository, RequestResponseLogRepository>();
        }

        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }

        public static void AddServicesForLangServices(this IServiceCollection services)
        {

        }
    }
}

