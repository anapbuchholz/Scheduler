using Scheduler.Infrastructure.Models;
using Scheduler.Infrastructure.Repositories;

namespace Scheduler.API.Services
{
    public interface ICompanyService
    {
        Task<Company?> GetCompanyAsync(Guid id);
    }

    public class CompanyService : ICompanyService
    {
        public readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<Company?> GetCompanyAsync(Guid id)
        {
            return await _companyRepository.GetCompanyAsync(id);
        }
    }
}
