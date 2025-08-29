using Scheduler.Application.Features.Shared.IO;
using System;

namespace Scheduler.Application.Features.UseCases.Company.GetCompany.UseCase
{
    public sealed class GetCompanyRequest : IRequest
    {
        public GetCompanyRequest(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
