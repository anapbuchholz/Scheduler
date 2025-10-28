using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Infrastructure.Data.Shared.SqlHelper.Services;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure.Data.Shared
{
    [ExcludeFromCodeCoverage]
    internal static class SharedExtensions
    {
        public static IServiceCollection AddShared(this IServiceCollection services)
        {
            services.AddScoped<ISqlService, SqlService>();
            return services;
        }
    }
}
