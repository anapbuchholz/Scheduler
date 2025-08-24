using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany;

namespace Scheduler.Application.Features.UseCases.Company
{
    internal static class CompanyExtensions
    {
        public static IServiceCollection AddCompanyFeatures(this IServiceCollection services)
        {
            services.AddRegisterCompany();
            return services;
        }
    }
}
