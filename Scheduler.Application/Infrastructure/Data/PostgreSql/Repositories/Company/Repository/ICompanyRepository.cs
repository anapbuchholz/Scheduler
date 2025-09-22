using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository
{
    internal interface ICompanyRepository
    {
        Task<CompanyEntity?> GetCompanyAsync(Guid id);
        Task<CompanyEntity?> GetCompanyByDocumentNumberAsync(string documentNumber);
        Task<List<CompanyEntity>> ListCompaniesAsync(string? name, string? documentNumber);
        Task RegisterCompanyAsync(CompanyEntity company);
    }
}
