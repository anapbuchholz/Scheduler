using Dapper;
using Scheduler.Application.Features.Shared.IO.Query;
using Scheduler.Application.Infrastructure.Data.Shared.Context;
using Scheduler.Application.Infrastructure.Data.Shared.Entity;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Data.Shared.Helpers.Sql
{
    internal sealed class SqlHelper(IDataContext context) : ISqlHelper
    {
        private readonly IDataContext _context = context;

        public async Task<int> CountAsync(string sql, object? param = default)
        {
            using var connection = _context.GetConnection();
            return await connection.QueryFirstAsync<int>(sql, param);
        }

        public async Task<int> ExecuteAsync(string sql, object? param = default)
        {
            using var connection = _context.GetConnection();
            return await connection.ExecuteAsync(sql, param);
        }

        public async Task<List<T>> SelectAsync<T>(string sql, object? param = default) where T : BaseEntity
        {
            using var connection = _context.GetConnection();
            var result = await connection.QueryAsync<T>(sql, param);
            return result.AsList();
        }

        public async Task<T?> SelectFirstOrDefaultAsync<T>(string sql, object? param = default) where T : BaseEntity
        {
            using var connection = _context.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public async Task<(bool Success, int RowsAffected)> ExecuteWithTransactionAsync(IDictionary<string, object?> commands)
        {
            int totalRowsAffected = 0;
            using var connection = _context.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var command in commands)
                {
                    var rowsAffected = await connection.ExecuteAsync(command.Key, command.Value, transaction);
                    totalRowsAffected += rowsAffected;
                    if (rowsAffected == 0)
                    {
                        transaction.Rollback();
                        return (false, 0);
                    }
                }
            }
            catch
            {
                transaction.Rollback();
                return (false, 0);
            }

            transaction.Commit();
            return (true, totalRowsAffected);
        }

        public async Task<PaginatedQueryResult<T>> SelectPaginated<T>(
            PaginationInput paginationInput,
            string selectStatement,
            string fromAndJoinsStatements,
            string whereStatement = "",
            bool independentWhereStatement = false,
            object? param = default
        ) where T : BaseEntity
        {
            selectStatement += " {0} {1} {2}";
            if (paginationInput.SearchParam == null && !independentWhereStatement)
                whereStatement = string.Empty;
            else if (!independentWhereStatement)
                whereStatement = whereStatement.Replace(PaginationInput.SEARCH_PARAM_REPLACE_CONST, paginationInput.SearchParam);

            var sqlCount = string.Format("SELECT COUNT(*) {0} {1}", fromAndJoinsStatements, whereStatement);
            var startRow = paginationInput.PageSize * (paginationInput.PageNumber - 1);
            var limitStatement = $"LIMIT {paginationInput.PageSize} OFFSET {startRow}";
            var sqlQuery = string.Format(selectStatement, fromAndJoinsStatements, whereStatement, limitStatement);

            using var connection = _context.GetConnection();
            var count = await connection.QueryFirstAsync<int>(sqlCount, param);
            var data = await connection.QueryAsync<T>(sqlQuery, param);

            var result = new PaginatedQueryResult<T>(data.AsList(), count);
            return result;
        }
    }
}
