using Scheduler.Application.Features.Shared.IO;

namespace Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase
{
    internal class RegisterCompanyInput : IInput
    {
        public string TradeName { get; set; }
        public string LegalName { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
