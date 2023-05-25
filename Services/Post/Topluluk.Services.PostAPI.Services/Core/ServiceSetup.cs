using System;
using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Topluluk.Services.PostAPI.Data.Implementation;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Data.Settings;
using Topluluk.Services.PostAPI.Services.Implementation;
using Topluluk.Services.PostAPI.Services.Interface;
using MongoDatabaseSettings = DBHelper.Connection.Mongo.MongoDatabaseSettings;

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
            services.AddScoped<IPostInteractionRepository,PostInteractionRepository>();   
            services.AddSingleton<IMongoClient>(new MongoClient("mongodb+srv://ismail:ismail@cluster0.psznbcu.mongodb.net/?retryWrites=true&w=majority"));
        }

        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<ITestPostService,TestPostService>();

        }

        public static void AddServicesForLangServices(this IServiceCollection services)
        {

        }
    }
}

