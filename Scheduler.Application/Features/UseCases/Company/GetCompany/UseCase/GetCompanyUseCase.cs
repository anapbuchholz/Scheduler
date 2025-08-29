using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.Company.GetCompany.UseCase
{
    internal sealed class GetCompanyUseCase(ICompanyRepository companyRepository) : IUseCase<GetCompanyRequest, Response>
    {
        private readonly ICompanyRepository _companyRepository = companyRepository;

        public async Task<Response> ExecuteAsync(GetCompanyRequest input)
        {
            var company = await _companyRepository.GetCompanyAsync(input.Id);

            if (company == null)
            {
                return Response.CreateNotFoundResponse();
            }

            return Response.CreateOkResponse(company);
        }
    }
}
