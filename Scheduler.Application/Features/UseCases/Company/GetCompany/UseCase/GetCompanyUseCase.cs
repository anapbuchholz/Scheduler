using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.Company.GetCompany.UseCase
{
    internal sealed class GetCompanyUseCase(
        ICompanyRepository companyRepository,
        IUserSession userSession) : IUseCase<GetCompanyRequest, Response>
    {
        private readonly ICompanyRepository _companyRepository = companyRepository;
        private readonly IUserSession _userSession = userSession;

        public async Task<Response> ExecuteAsync(GetCompanyRequest input)
        {
            try
            {
                if (!_userSession.IsAdmin)
                {
                    return Response.CreateForbiddenResponse();
                }

                var company = await _companyRepository.GetCompanyAsync(input.Id);

                if (company == null)
                {
                    return Response.CreateNotFoundResponse();
                }

                return Response.CreateOkResponse(company);
            }
            catch (Exception ex)
            {
                return Response.CreateInternalErrorResponse($"{nameof(GetCompanyUseCase)}->{nameof(ExecuteAsync)}: {ex.Message}");

            }
        }

    }
}
