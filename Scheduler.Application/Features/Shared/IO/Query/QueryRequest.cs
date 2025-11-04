using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.Shared.IO.Query
{
    [ExcludeFromCodeCoverage]
    public abstract class QueryRequest(int pageNumber, int pageSize, string? searchParam = null) : IRequest
    {
        public PaginationInput PaginationParameters { get; } = PaginationInput.Create(pageNumber, pageSize, searchParam);
    }
}
