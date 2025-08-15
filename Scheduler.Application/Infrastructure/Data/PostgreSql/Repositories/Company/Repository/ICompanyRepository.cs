using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository
{
    internal interface ICompanyRepository
    {
        Task<CompanyEntity?> GetCompanyAsync(Guid id);
        Task RegisterCompanyAsync(CompanyEntity company);
    }
}
