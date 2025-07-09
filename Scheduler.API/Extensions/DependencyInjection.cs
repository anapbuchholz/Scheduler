using Npgsql;
using Scheduler.API.Services;
using Scheduler.Infrastructure.Data;
using Scheduler.Infrastructure.Repositories;
using System.Data;

namespace Scheduler.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbConnection>(sp =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                return new NpgsqlConnection(connectionString);
            });

            services.AddScoped<DapperContext>();

            services.AddScoped<ICompanyRepository, CompanyRepository>();
            
            services.AddScoped<ICompanyService, CompanyService>();

            return services;
        }
    }
}
