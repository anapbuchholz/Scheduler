using System.Collections.Generic;

namespace Scheduler.Application.Infrastructure.Data.Shared.SqlHelper.Pagination
{
    internal sealed class PaginatedQueryResult<T> where T : class
    {
        public int TotalCount { get; }
        public IEnumerable<T> Results { get; }

        public PaginatedQueryResult()
        {
            Results = [];
            TotalCount = 0;
        }

        public PaginatedQueryResult(IEnumerable<T> results, int totalCount)
        {
            Results = results;
            TotalCount = totalCount;
        }
    }
}
