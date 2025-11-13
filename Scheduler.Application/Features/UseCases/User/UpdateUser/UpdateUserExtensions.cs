using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.User.UpdateUser.UseCase;
using Scheduler.Application.Features.UseCases.User.UpdateUser.Validators;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.UpdateUser
{
    [ExcludeFromCodeCoverage]
    internal static class UpdateUserExtensions
    {
        public static IServiceCollection AddUpdateUser(this IServiceCollection services)
        {
            services.AddScoped<IUseCase<UpdateUserRequest, Response>, UpdateUserUseCase>();
            services.AddScoped<IRequestValidator<UpdateUserRequest>, UpdateUserValidator>();
            return services;
        }
    }
}