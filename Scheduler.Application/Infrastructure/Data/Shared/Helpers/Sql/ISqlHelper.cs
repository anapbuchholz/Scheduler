using Scheduler.Application.Features.Shared.IO.Query;
using Scheduler.Application.Infrastructure.Data.Shared.Entity;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Data.Shared.Helpers.Sql
{
    internal interface ISqlHelper
    {
        Task<List<T>> SelectAsync<T>(string sql, object @params) where T : BaseEntity;
        Task<T?> SelectFirstOrDefaultAsync<T>(string sql, object? param = default) where T : BaseEntity;
        Task<int> CountAsync(string sql, object @params);
        Task<int> ExecuteAsync(string sql, object @params);
        Task<(bool Success, int RowsAffected)> ExecuteWithTransactionAsync(IDictionary<string, object?> commands);
        Task<PaginatedQueryResult<T>> SelectPaginated<T>(
            PaginationInput paginationInput,
            string selectStatement,
            string fromAndJoinsStatements,
            string whereStatement = "",
            bool independentWhereStatement = false,
            object? param = default) where T : BaseEntity;
    }
}
