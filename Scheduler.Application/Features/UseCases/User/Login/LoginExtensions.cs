using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.User.Login.UseCase;
using Scheduler.Application.Features.UseCases.User.Login.Validators;

namespace Scheduler.Application.Features.UseCases.User.Login
{
    internal static class LoginExtensions
    {
        public static IServiceCollection AddLogin(this IServiceCollection services)
        {
            services.AddScoped<IUseCase<LoginRequest, Response>, LoginUseCase>();
            services.AddScoped<IRequestValidator<LoginRequest>, LoginValidator>();
            return services;
        }
    }
}
