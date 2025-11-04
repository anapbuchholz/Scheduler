using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Query;

namespace Scheduler.Application.Features.UseCases.Company.ListCompanies.UseCase
{
    public sealed class ListCompaniesRequest(string? name, string? documentNumber, int pageNumber, int pageSize) : QueryRequest(pageNumber, pageSize)
    {
        public string? Name { get; set; } = name;
        public string? DocumentNumber { get; set; } = documentNumber;
    }
}
