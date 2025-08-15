using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.Validators;

namespace Scheduler.Application.Features.UseCases.Company.RegisterCompany
{
    internal static class RegisterCompanyExtensions
    {
        public static IServiceCollection AddRegisterCompany(this IServiceCollection services)
        {
            services.AddScoped<IRequestValidator<RegisterCompanyInput>, RegisterCompanyValidator>();
            services.AddScoped<IUseCase<RegisterCompanyInput, Output>, RegisterCompanyUseCase>();
            return services;
        }
    }
}
