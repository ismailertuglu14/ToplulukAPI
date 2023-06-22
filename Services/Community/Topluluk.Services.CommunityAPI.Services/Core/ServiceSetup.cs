using System;
using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Topluluk.Services.CommunityAPI.Data.Implementation;
using Topluluk.Services.CommunityAPI.Data.Interface;
using Topluluk.Services.CommunityAPI.Data.Settings;
using Topluluk.Services.CommunityAPI.Services.Implementation;
using Topluluk.Services.CommunityAPI.Services.Interface;

namespace Topluluk.Services.CommunityAPI.Services.Core
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
            services.AddSingleton<IDbConfiguration, CommunityAPIDbSettings>();
            services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
            services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>();
            // services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICommunityRepository, CommunityRepository>();
            services.AddScoped<ICommunityParticipiantRepository, CommunityParticipiantRepository>();
            
        }

        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddTransient<ICommunityService, CommunityService>();
            services.AddTransient<ICommunityImageService, CommunityImageService>();
            services.AddTransient<IParticipiantService,ParticipiantService>();
        }
    }
}

