using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.UseCases.Company;

namespace Scheduler.Application.Features
{
    internal static class FeaturesExtensions
    {
        public static IServiceCollection AddFeatures(this IServiceCollection services)
        {
            services.AddCompanyFeatures();
            return services;
        }
    }
}
