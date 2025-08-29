using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.Company.GetCompany.UseCase;

namespace Scheduler.Application.Features.UseCases.Company.GetCompany
{
    internal static class GetCompanyExtensions
    {
        public static IServiceCollection AddGetCompany(this IServiceCollection services)
        {
            services.AddScoped<IUseCase<GetCompanyRequest, Response>, GetCompanyUseCase>();
            return services;
        }
    }
}
