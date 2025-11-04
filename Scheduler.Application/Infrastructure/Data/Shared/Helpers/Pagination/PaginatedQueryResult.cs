using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination
{
    [ExcludeFromCodeCoverage]
    internal sealed class PaginatedQueryResult<T> where T : class
    {
        public int TotalCount { get; }
        public List<T> Results { get; }

        public PaginatedQueryResult()
        {
            Results = [];
            TotalCount = 0;
        }

        public PaginatedQueryResult(List<T> results, int totalCount)
        {
            Results = results;
            TotalCount = totalCount;
        }
    }
}
