using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.DeleteUser.UseCase;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.DeleteUser
{
    [ExcludeFromCodeCoverage]
    internal static class DeleteUserExtensions
    {
        public static IServiceCollection AddDeleteUser(this IServiceCollection services)
        {
            services.AddScoped<IUseCase<DeleteUserRequest, Response>, DeleteUserUseCase>();
            return services;
        }
    }
}
