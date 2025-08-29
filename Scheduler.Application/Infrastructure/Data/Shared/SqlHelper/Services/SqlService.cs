using Dapper;
using Scheduler.Application.Features.Shared.IO.Query;
using Scheduler.Application.Infrastructure.Data.Shared.Context;
using Scheduler.Application.Infrastructure.Data.Shared.SqlHelper.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Data.Shared.SqlHelper.Services
{
    internal sealed class SqlService(IDataContext context) : ISqlService
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

        public async Task<IEnumerable<T>> SelectAsync<T>(string sql, object? param = default)
        {
            using var connection = _context.GetConnection();
            return await connection.QueryAsync<T>(sql, param);
        }

        public async Task<T> SelectSingleAsync<T>(string sql, object? param = default)
        {
            using var connection = _context.GetConnection();
            return await connection.QuerySingleAsync<T>(sql, param);
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

        public async Task<PaginatedQueryResult<T>> SelectPaginated<T>(QueryRequest queryRequest, string selectStatement, string fromAndJoinsStatements, string whereStatement = "", bool independentWhereStatement = false) where T : class
        {
            selectStatement += " {0} {1} {2}";
            if (queryRequest.SearchParam == null && !independentWhereStatement)
                whereStatement = string.Empty;
            else if (!independentWhereStatement)
                whereStatement = whereStatement.Replace("@search_param", queryRequest.SearchParam);

            var sqlCount = string.Format("SELECT COUNT(*) {0} {1}", fromAndJoinsStatements, whereStatement);
            var startRow = queryRequest.PageSize * (queryRequest.PageNumber - 1);
            var limitStatement = $"LIMIT {startRow}, {queryRequest.PageSize}";
            var sqlQuery = string.Format(selectStatement, fromAndJoinsStatements, whereStatement, limitStatement);

            using var connection = _context.GetConnection();
            var count = await connection.QueryFirstAsync<int>(sqlCount);
            var data = await connection.QueryAsync<T>(sqlQuery);

            var result = new PaginatedQueryResult<T>(data, count);
            return result;
        }
    }
}
