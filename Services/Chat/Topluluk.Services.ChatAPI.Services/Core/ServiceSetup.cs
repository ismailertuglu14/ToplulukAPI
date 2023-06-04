using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Topluluk.Services.ChatAPI.Data.Implementation;
using Topluluk.Services.ChatAPI.Data.Interface;
using Topluluk.Services.ChatAPI.Data.Settings;
using Topluluk.Services.ChatAPI.Services.Implementation;
using Topluluk.Services.ChatAPI.Services.Interface;

namespace Topluluk.Services.ChatAPI.Services.Core;

public static class ServiceSetup
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        AddServicesForRepository(services);
        AddServicesForServices(services);
    }

    public static void AddServicesForRepository(this IServiceCollection services)
    {
        services.AddSingleton<IDbConfiguration, ChatAPIDbSettings>();
        services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
        services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>();
        services.AddScoped<IChatRepository,ChatRepository>();
    }

    public static void AddServicesForServices(this IServiceCollection services)
    {
        services.AddTransient<IChatService, ChatService>();
        services.AddSignalR();
    }

}