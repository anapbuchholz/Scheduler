using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase;

namespace Scheduler.Application.Features.UseCases.Company.RegisterCompany.Validators
{
    internal class RegisterCompanyValidator : IRequestValidator<RegisterCompanyInput>
    {
        public Task<RequestValidationModel> Validate(RegisterCompanyInput request)
        {
            throw new NotImplementedException();
        }
    }
}
