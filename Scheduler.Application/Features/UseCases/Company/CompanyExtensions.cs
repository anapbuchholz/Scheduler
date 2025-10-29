using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.UseCases.Company.GetCompany;
using Scheduler.Application.Features.UseCases.Company.ListCompanies;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.Company
{
    [ExcludeFromCodeCoverage]
    internal static class CompanyExtensions
    {
        public static IServiceCollection AddCompanyFeatures(this IServiceCollection services)
        {
            services.AddRegisterCompany();
            services.AddGetCompany();
            services.AddListCompanies();
            return services;
        }
    }
}
