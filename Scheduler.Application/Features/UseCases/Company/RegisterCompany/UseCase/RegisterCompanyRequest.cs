using Scheduler.Application.Features.Shared.IO;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase
{
    [ExcludeFromCodeCoverage]
    public class RegisterCompanyRequest : IRequest
    {
        public string? TradeName { get; set; }
        public string? LegalName { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
