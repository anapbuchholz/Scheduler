using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.GetUser.UseCase;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.GetUser
{
    [ExcludeFromCodeCoverage]
    internal static class GetUserExtensions
    {
        public static IServiceCollection AddGetUser(this IServiceCollection services)
        {
            services.AddScoped<IUseCase<GetUserRequest, Response>, GetUserUseCase>();
            return services;
        }
    }
}
