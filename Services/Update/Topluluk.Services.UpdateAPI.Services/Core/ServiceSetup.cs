using DBHelper.BaseDto;
using DBHelper.Connection;
using DBHelper.Connection.Mongo;
using Microsoft.Extensions.DependencyInjection;
using Topluluk.Services.UpdateAPI.Data.Implementation;
using Topluluk.Services.UpdateAPI.Data.Interface;
using Topluluk.Services.UpdateAPI.Data.Settings;
using Topluluk.Services.UpdateAPI.Services.Implementation;
using Topluluk.Services.UpdateAPI.Services.Interface;

namespace Topluluk.Services.UpdateAPI.Services.Core;

public static class ServiceSetup
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        AddServicesForRepository(services);
        AddServicesForServices(services);
    }

    public static void AddServicesForRepository(this IServiceCollection services)
    {
        services.AddSingleton<IDbConfiguration, UpdateAPIDbSettings>();
        services.AddSingleton<IConnectionFactory, MongoConnectionFactory>();
        services.AddSingleton<IBaseDatabaseSettings, MongoDatabaseSettings>();
        services.AddScoped<IUpdateRepository, UpdateRepository>();
    }

    public static void AddServicesForServices(this IServiceCollection services)
    {
        services.AddTransient<IUpdateService, UpdateService>();
    }
}
