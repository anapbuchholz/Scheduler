namespace Scheduler.Application.Features.Shared.IO.Query
{
    public abstract class QueryInput : IInput
    {
        public QueryInput(QueryRequest request)
        {
            Request = request;
        }

        public QueryRequest Request { get; }
    }
}
