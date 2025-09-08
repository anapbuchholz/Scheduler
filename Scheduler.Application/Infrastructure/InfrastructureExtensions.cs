using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Infrastructure.Authentication;
using Scheduler.Application.Infrastructure.Authorization;
using Scheduler.Application.Infrastructure.Data.PostgreSql;
using Scheduler.Application.Infrastructure.Data.Shared;

namespace Scheduler.Application.Infrastructure
{
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
