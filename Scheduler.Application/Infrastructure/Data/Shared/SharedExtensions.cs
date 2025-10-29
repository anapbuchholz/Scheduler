using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Sql;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure.Data.Shared
{
    [ExcludeFromCodeCoverage]
    internal static class SharedExtensions
    {
        public static IServiceCollection AddShared(this IServiceCollection services)
        {
            services.AddScoped<ISqlHelper, SqlHelper>();
            return services;
        }
    }
}
