using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.Company.ListCompanies.UseCase;

namespace Scheduler.Application.Features.UseCases.Company.ListCompanies
{
    internal static class ListCompaniesExtensions
    {
        public static IServiceCollection AddListCompanies(this IServiceCollection services)
        {
            services.AddScoped<IUseCase<ListCompaniesRequest, Response>, ListCompaniesUseCase>();
            return services;
        }
    }
}
