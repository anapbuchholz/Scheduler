using Dapper;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.Shared.Context;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository
{
    internal sealed class CompanyRepository(IDataContext context) : ICompanyRepository
    {
        private readonly IDataContext _context = context;

        public async Task<CompanyEntity?> GetCompanyAsync(Guid id)
        {
            var query = CompanySqlConstants.SELECT_COMPANY_BY_ID;

            var connection = _context.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<CompanyEntity>(query, new { Id = id });
        }

        public async Task<CompanyEntity?> GetCompanyByDocumentNumberAsync(string documentNumber)
        {
            var query = CompanySqlConstants.SELECT_COMPANY_BY_DOCUMENT_NUMBER;

            var connection = _context.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<CompanyEntity>(query, new { DocumentNumber = documentNumber });
        }


        public async Task RegisterCompanyAsync(CompanyEntity company)
        {
            var command = CompanySqlConstants.INSERT_COMPANY;

            var connection = _context.GetConnection();
            await connection.ExecuteAsync(command, company);
        }
    }
}
