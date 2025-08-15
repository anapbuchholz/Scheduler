using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.Shared.ValueObjects;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase;

namespace Scheduler.Application.Features.UseCases.Company.RegisterCompany.Validators
{
    internal class RegisterCompanyValidator : IRequestValidator<RegisterCompanyRequest>
    {
        public Task<RequestValidationModel> Validate(RegisterCompanyRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.TradeName))
            {
                errors.Add("O nome fantasia deve ser informado.");
            }

            if (string.IsNullOrWhiteSpace(request.LegalName))
            {
                errors.Add("A razão social deve ser informada");
            }

            var email = EmailValueObject.Create(request.Email!);
            if (!email.IsNullOrEmpty)
            {
                if (email.IsValidEmail)
                {
                    errors.Add("O E-mail informado é inválido.");
                }
            }

            var documentNumber = DocumentNumberValueObject.Create(request.DocumentNumber!);
            if (documentNumber.IsNullOrWhiteSpace)
            {
                errors.Add("O Número do documento deve ser informado.");
            }
            if (!documentNumber.IsDigitOnly)
            {
                errors.Add("O CNPJ/CPF deve conter apenas números.");
            }
            if (documentNumber.IsGreaterThanMaxLength)
            {
                errors.Add($"O CNPJ/CPF deve conter no máximo {documentNumber.MaxLength} dígitos.");
            }
            if (documentNumber.IsLessThanMinLength)
            {
                errors.Add($"O CNPJ/CPF deve conter no mínimo {documentNumber.MinLength} dígitos.");
            }

            return Task.FromResult(new RequestValidationModel(errors));
        }
    }
}
