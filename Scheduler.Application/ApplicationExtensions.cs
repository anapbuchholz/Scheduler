using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features;
using Scheduler.Application.Infrastructure;

namespace Scheduler.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddInfrastructure();
            services.AddFeatures();
            return services;
        }
    }
}
