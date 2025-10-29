using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Infrastructure.Authentication;
using Scheduler.Application.Infrastructure.Authorization;
using Scheduler.Application.Infrastructure.Data.PostgreSql;
using Scheduler.Application.Infrastructure.Data.Shared;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure
{
    [ExcludeFromCodeCoverage]
    internal static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddShared();
            services.AddPostgreSql();
            services.AddUserSession();
            services.AddFireBaseAuthentication();
            return services;
        }
    }
}
