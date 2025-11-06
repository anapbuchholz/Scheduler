using Scheduler.Application.Features.Shared.IO.Query;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.Company.ListCompanies.UseCase
{
    [ExcludeFromCodeCoverage]
    public sealed class ListCompaniesRequest(string? name, string? documentNumber, int pageNumber, int pageSize) : QueryRequest(pageNumber, pageSize)
    {
        public string? Name { get; set; } = name;
        public string? DocumentNumber { get; set; } = documentNumber;
    }
}
