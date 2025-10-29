using Dapper;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Sql;
using System;
using System.Collections.Generic;
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

        public async Task<List<CompanyEntity>> ListCompaniesAsync(string? name, string? documentNumber)
        {
            var query = CompanySqlConstants.LIST_COMPANIES;
            var parameters = new DynamicParameters();
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                query += " AND (LOWER(trade_name) LIKE @Name OR LOWER(legal_name) LIKE @Name)";
                parameters.Add("Name", $"%{name.ToLower()}%");
            }
            if (!string.IsNullOrWhiteSpace(documentNumber))
            {
                query += " AND tax_id = @DocumentNumber";
                parameters.Add("DocumentNumber", documentNumber);
            }
            
            var companyList = await _sqlHelper.SelectAsync<CompanyEntity>(query, parameters);
            return companyList.AsList();
        }

        public async Task RegisterCompanyAsync(CompanyEntity company)
        {
            var command = CompanySqlConstants.INSERT_COMPANY;
            await _sqlHelper.ExecuteAsync(command, company);
        }
    }
}
