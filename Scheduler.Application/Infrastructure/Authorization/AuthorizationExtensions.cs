using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Infrastructure.Authorization.Implementations;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure.Authorization
{
    [ExcludeFromCodeCoverage]
    internal static class AuthorizationExtensions
    {
        public static IServiceCollection AddUserSession(this IServiceCollection services)
        {
            services.AddScoped<UserSession>();
            services.AddScoped<IUserSession>(provider => provider.GetRequiredService<UserSession>());
            services.AddScoped<IUserSessionSet>(provider => provider.GetRequiredService<UserSession>());

            return services;
        }
    }
}
