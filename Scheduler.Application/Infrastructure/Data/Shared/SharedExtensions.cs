using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Infrastructure.Data.Shared.SqlHelper.Services;

namespace Scheduler.Application.Infrastructure.Data.Shared
{
    internal static class SharedExtensions
    {
        public static IServiceCollection AddShared(this IServiceCollection services)
        {
            services.AddScoped<ISqlService, SqlService>();
            return services;
        }
    }
}
