using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using Scheduler.Application.Infrastructure.Data.Shared.Context;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql
{
    [ExcludeFromCodeCoverage]
    internal static class PostgreSqlExtensions
    {
        public static IServiceCollection AddPostgreSql(this IServiceCollection services)
        {
            DapperMappingInitializer.Initialize();
            services.AddScoped<IDataContext, PostgreSqlDataContext>();
            AddRepositories(services);
            return services;
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
