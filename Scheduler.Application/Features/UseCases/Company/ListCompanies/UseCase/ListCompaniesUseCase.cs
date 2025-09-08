using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.Company.ListCompanies.UseCase
{
    internal sealed class ListCompaniesUseCase(
        ICompanyRepository companyRepository,
        IUserSession userSession) : IUseCase<ListCompaniesRequest, Response>
    {
        private readonly ICompanyRepository _companyRepository = companyRepository;
        private readonly IUserSession _userSession = userSession;

        public async Task<Response> ExecuteAsync(ListCompaniesRequest input)
        {
            try
            {
                if (!_userSession.IsAdmin)
                {
                    return Response.CreateForbiddenResponse();
                }

                var companies = await _companyRepository.ListCompaniesAsync(input.Name, input.DocumentNumber);
                return Response.CreateOkResponse(companies);
            }
            catch (Exception ex)
            {
                return Response.CreateInternalErrorResponse($"{nameof(ListCompaniesUseCase)}->{nameof(ExecuteAsync)}: {ex.Message}");
            }            
        }
    }
}
