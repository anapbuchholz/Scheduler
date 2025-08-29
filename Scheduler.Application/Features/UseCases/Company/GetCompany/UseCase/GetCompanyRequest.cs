using Scheduler.Application.Features.Shared.IO;
using System;

namespace Scheduler.Application.Features.UseCases.Company.GetCompany.UseCase
{
    internal sealed class GetCompanyRequest : IRequest
    {
        public Guid Id { get; set; }
    }
}
