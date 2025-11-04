using Dapper;
using Scheduler.Application.Features.Shared.IO.Query;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Sql;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository
{
    internal sealed class CompanyRepository(ISqlHelper sqlhelper) : ICompanyRepository
    {
        private readonly ISqlHelper _sqlHelper = sqlhelper;

        public async Task<CompanyEntity?> GetCompanyAsync(Guid id)
        {
            var query = CompanySqlConstants.SELECT_COMPANY_BY_ID;
            return await _sqlHelper.SelectFirstOrDefaultAsync<CompanyEntity>(query, new { Id = id });
        }

        public async Task<CompanyEntity?> GetCompanyByDocumentNumberAsync(string documentNumber)
        {
            var query = CompanySqlConstants.SELECT_COMPANY_BY_DOCUMENT_NUMBER;
            return await _sqlHelper.SelectFirstOrDefaultAsync<CompanyEntity>(query, new { DocumentNumber = documentNumber });
        }

        public async Task<PaginatedQueryResult<CompanyEntity>> ListCompaniesAsync(string? name, string? documentNumber, PaginationInput paginationInput)
        {
            var whereClause = CompanySqlConstants.ListCompaniesPaginationConstants.LIST_COMPANIES_WHERE_STATEMENT;
            var parameters = new DynamicParameters();            
            if (!string.IsNullOrWhiteSpace(name))
            {
                whereClause += " AND (LOWER(trade_name) LIKE @Name OR LOWER(legal_name) LIKE @Name)";
                parameters.Add("Name", $"%{name.ToLower()}%");
            }
            if (!string.IsNullOrWhiteSpace(documentNumber))
            {
                whereClause += " AND tax_id = @DocumentNumber";
                parameters.Add("DocumentNumber", documentNumber);
            }
            
            var companyList = await _sqlHelper.SelectPaginated<CompanyEntity>(
                paginationInput,
                CompanySqlConstants.ListCompaniesPaginationConstants.LIST_COMPANIES_SELECT_STATEMENT,
                CompanySqlConstants.ListCompaniesPaginationConstants.LIST_COMPANIES_FROM_STATEMENT,
                whereClause,
                true,
                parameters);

            return companyList;
        }

        public async Task RegisterCompanyAsync(CompanyEntity company)
        {
            var command = CompanySqlConstants.INSERT_COMPANY;
            await _sqlHelper.ExecuteAsync(command, company);
        }
    }
}
