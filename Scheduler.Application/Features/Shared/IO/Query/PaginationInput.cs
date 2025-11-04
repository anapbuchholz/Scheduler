namespace Scheduler.Application.Features.Shared.IO.Query
{
    public sealed class PaginationInput
    {
        private static readonly int MAX_DEFAULT_PAGE_SIZE = 100;
        private static readonly int MIN_DEFAULT_PAGE_NUMBER = 1;

        public const string SEARCH_PARAM_REPLACE_CONST = "@search_param";

        public int PageNumber { get; }
        public int PageSize { get; }
        public string? SearchParam { get; }

        private PaginationInput(in int pageNumber, in int pageSize, in string? searchParam)
        {
            PageNumber = pageNumber < MIN_DEFAULT_PAGE_NUMBER ? MIN_DEFAULT_PAGE_NUMBER : pageNumber;
            PageSize = pageSize > MAX_DEFAULT_PAGE_SIZE || pageSize == 0 ? MAX_DEFAULT_PAGE_SIZE : pageSize;
            SearchParam = searchParam;
        }

        public static PaginationInput Create(in int pageNumber, in int pageSize, in string? searchParam) => new(pageNumber, pageSize, searchParam);
    }
}
