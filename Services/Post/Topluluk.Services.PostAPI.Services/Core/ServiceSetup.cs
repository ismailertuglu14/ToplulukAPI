using System;
using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Topluluk.Services.PostAPI.Data.Implementation;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Data.Settings;
using Topluluk.Services.PostAPI.Services.Implementation;
using Topluluk.Services.PostAPI.Services.Interface;

namespace Topluluk.Services.PostAPI.Services.Core
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
            services.AddSingleton<IDbConfiguration, PostAPIDbSettings>();
            services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
            services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>();
            // services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostCommentRepository, PostCommentRepository>();
            services.AddScoped<ISavedPostRepository, SavedPostRepository>();
            // services.AddTransient<IErrorRepository, ErrorRepository>();
            // services.AddTransient<IRequestResponseLogRepository, RequestResponseLogRepository>();
        }

        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<ISavedPostRepository, SavedPostRepository>();

        }

        public static void AddServicesForLangServices(this IServiceCollection services)
        {

        }
    }
}

