namespace Scheduler.Application.Features.Shared.IO.Query
{
    public sealed class QueryRequest
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public string? SearchParam { get; }

        private QueryRequest(in int pageNumber, in int pageSize, in string? searchParam)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 || pageSize == 0 ? 10 : pageSize;
            SearchParam = searchParam;
        }

        public static QueryRequest Create(in int pageNumber, in int pageSize, in string? searchParam) => new QueryRequest(pageNumber, pageSize, searchParam);
    }
}
