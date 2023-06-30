using System;
using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Topluluk.Services.PostAPI.Data.Implementation;
using Topluluk.Services.PostAPI.Data.Interface;
using Topluluk.Services.PostAPI.Data.Settings;
using Topluluk.Services.PostAPI.Model.Entity;
using Topluluk.Services.PostAPI.Services.Implementation;
using Topluluk.Services.PostAPI.Services.Interface;
using Topluluk.Shared.Middleware;
using MongoDatabaseSettings = DBHelper.Connection.Mongo.MongoDatabaseSettings;

namespace Topluluk.Services.PostAPI.Services.Core
{
    public static class ServiceSetup
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddServicesForRepository(services, configuration);
            AddServicesForServices(services);
        }

        public static void AddServicesForRepository(this IServiceCollection services,IConfiguration configuration)
        {
            
            services.AddSingleton<IDbConfiguration, PostAPIDbSettings>();
            services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
            services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>();
            // services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentInteractionRepository, CommentInteractionRepository>();
            services.AddScoped<IPostCommentRepository, PostCommentRepository>();
            services.AddScoped<ISavedPostRepository, SavedPostRepository>();
            services.AddScoped<IPostInteractionRepository,PostInteractionRepository>();   
            services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDB")));
        }

        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IPostCommentService,PostCommentService>();
            services.AddTransient<ITestPostService,TestPostService>();

        }

        public static void AddServicesForMiddlewares(IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}

