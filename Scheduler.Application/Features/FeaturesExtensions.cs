using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.UseCases.Company;
using Scheduler.Application.Features.UseCases.User;

namespace Scheduler.Application.Features
{
    internal static class FeaturesExtensions
    {
        public static IServiceCollection AddFeatures(this IServiceCollection services)
        {
            services.AddCompanyFeatures();
            services.AddUserFeatures();
            return services;
        }
    }
}
