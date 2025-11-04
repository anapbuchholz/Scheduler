using Scheduler.Application.Features.Shared.IO.Query;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository
{
    internal interface ICompanyRepository
    {
        Task<CompanyEntity?> GetCompanyAsync(Guid id);
        Task<CompanyEntity?> GetCompanyByDocumentNumberAsync(string documentNumber);
        Task<PaginatedQueryResult<CompanyEntity>> ListCompaniesAsync(string? name, string? documentNumber, PaginationInput paginationInput);
        Task RegisterCompanyAsync(CompanyEntity company);
    }
}
