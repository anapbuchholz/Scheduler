namespace Scheduler.Application.Features.Shared.IO.Query
{
    public abstract class QueryInput : IRequest
    {
        public QueryInput(QueryRequest request)
        {
            Request = request;
        }

        public QueryRequest Request { get; }
    }
}
