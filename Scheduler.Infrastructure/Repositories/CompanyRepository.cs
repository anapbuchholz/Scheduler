using Dapper;
using Scheduler.Infrastructure.Data;
using Scheduler.Infrastructure.Models;

namespace Scheduler.Infrastructure.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company?> GetCompanyAsync(Guid id);
    }

    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;

        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetCompanyAsync(Guid id)
        {
            const string query = "SELECT * FROM scheduler.companies WHERE id = @Id";

            var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Company>(query, new {Id = id});
        }
    }
}
