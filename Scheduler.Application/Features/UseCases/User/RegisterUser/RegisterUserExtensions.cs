using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
using Scheduler.Application.Features.UseCases.User.RegisterUser.Validators;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.RegisterUser
{
    [ExcludeFromCodeCoverage]
    internal static class RegisterUserExtensions
    {
        public static IServiceCollection AddRegisterUser(this IServiceCollection services)
        {
            services.AddScoped<IUseCase<RegisterUserRequest, Response>, RegisterUserUseCase>();
            services.AddScoped<IRequestValidator<RegisterUserRequest>, RegisterUserValidator>();
            return services;
        }
    }
}
