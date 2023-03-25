using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topluluk.Services.FeedAPI.Services.Implementation;
using Topluluk.Services.FeedAPI.Services.Interface;

namespace Topluluk.Services.FeedAPI.Services.Core
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
            // services.AddTransient<IUnitOfWork, UnitOfWork>();
            // services.AddTransient<IErrorRepository, ErrorRepository>();
            // services.AddTransient<IRequestResponseLogRepository, RequestResponseLogRepository>();
        }

        public static void AddServicesForServices(this IServiceCollection services)
        {
            services.AddTransient<IFeedService, FeedService>();
        }



    }
}
