using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.ListUsers.UseCase;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.ListUsers
{
    [ExcludeFromCodeCoverage]
    internal static class ListUsersExtensions
    {
        public static IServiceCollection AddListUsers(this IServiceCollection services)
        {
            services.AddScoped<IUseCase<ListUsersRequest, Response>, ListUsersUseCase>();
            return services;
        }
    }
}
