using System;
using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using DBHelper.Repository;
using DBHelper.Repository.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Topluluk.Services.User.Data.Implementation;
using Topluluk.Services.User.Data.Interface;
using Topluluk.Services.User.Data.Settings;
using Topluluk.Services.User.Model.Entity;
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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserFollowRepository, UserFollowRepository>();
            services.AddScoped<IBlockedUserRepository, BlockedUserRepository>();
            services.AddScoped<IUserFollowRequestRepository, UserFollowRequestRepository>();
            services.AddSingleton<IRedisRepository, RedisCacheRepository>();
        }
        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<IImageService, ImageService>();
        }
    

    }
}

