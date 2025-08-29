using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.Company.GetCompany.UseCase
{
    internal sealed class GetCompanyUseCase : IUseCase<GetCompanyRequest, Response>
    {
        public Task<Response> Execute(GetCompanyRequest input)
        {
            throw new NotImplementedException();
        }
    }
}
