using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.Company.ListCompanies.UseCase
{
    internal sealed class ListCompaniesUseCase(ICompanyRepository companyRepository) : IUseCase<ListCompaniesRequest, Response>
    {
        private readonly ICompanyRepository _companyRepository = companyRepository;

        public async Task<Response> ExecuteAsync(ListCompaniesRequest input)
        {
            var companies = await _companyRepository.ListCompaniesAsync(input.Name, input.DocumentNumber);
            return Response.CreateOkResponse(companies);
        }
    }
}
