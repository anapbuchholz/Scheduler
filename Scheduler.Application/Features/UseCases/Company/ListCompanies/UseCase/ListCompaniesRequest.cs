using Scheduler.Application.Features.Shared.IO;

namespace Scheduler.Application.Features.UseCases.Company.ListCompanies.UseCase
{
    public sealed class ListCompaniesRequest : IRequest
    {
        public ListCompaniesRequest(string? name, string? documentNumber)
        {
            Name = name;
            DocumentNumber = documentNumber;
        }

        public string? Name { get; set; }
        public string? DocumentNumber { get; set; }
    }
}
