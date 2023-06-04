using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Topluluk.Services.EventAPI.Data.Implementation;
using Topluluk.Services.EventAPI.Data.Interface;
using Topluluk.Services.EventAPI.Data.Settings;
using Topluluk.Services.EventAPI.Services.Implementation;
using Topluluk.Services.EventAPI.Services.Interface;

namespace Topluluk.Services.EventAPI.Services.Core
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
            services.AddSingleton<IDbConfiguration, EventAPIDbSettings>();
            services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
            services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventCommentRepository, EventCommentRepository>();
            services.AddScoped<IEventAttendeesRepository,EventAttendeesRepository>();
        }

        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddTransient<IEventService, EventService>();
            services.AddScoped<IEventCommentService, EventCommentService>();

        }
    }
}

