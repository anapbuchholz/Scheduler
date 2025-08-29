using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.Shared.ValueObjects;
using Scheduler.Application.Features.UseCases.Company.RegisterCompany.UseCase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.Company.RegisterCompany.Validators
{
    internal class RegisterCompanyValidator : IRequestValidator<RegisterCompanyRequest>
    {
        private readonly int _tradeNameMaxLength = 255;
        private readonly int _legalNameMaxLength = 255;
        public Task<RequestValidationModel> Validate(RegisterCompanyRequest request)
        {
            var errors = new List<string>();

            #region TradeName
            var isTradeNameEmpty = string.IsNullOrWhiteSpace(request.TradeName);
            if (isTradeNameEmpty)
            {
                errors.Add("O nome fantasia deve ser informado.");
            }
            if (!isTradeNameEmpty && request.TradeName?.Length > _tradeNameMaxLength)
            {
                errors.Add($"O nome fantasia deve conter no máximo {_tradeNameMaxLength} caracteres.");
            }
            #endregion

            #region LegalName
            var isLegalNameEmpty = string.IsNullOrWhiteSpace(request.LegalName);
            if (isLegalNameEmpty)
            {
                errors.Add("A razão social deve ser informada");
            }
            if (!isLegalNameEmpty && request.LegalName?.Length > _legalNameMaxLength)
            {
                errors.Add($"A razão social deve conter no máximo {_legalNameMaxLength} caracteres.");
            }
            #endregion

            #region Email
            var email = EmailValueObject.Create(request.Email!);
            if (!email.IsNullOrEmpty)
            {
                if (email.IsValidEmail)
                {
                    errors.Add("O E-mail informado é inválido.");
                }

                if(email.IsGreaterThanMaxLength)
                {
                    errors.Add($"O E-mail deve conter no máximo {email.MaxLength} caracteres.");
                }

                if (email.IsLessThanMinLength)
                {
                    errors.Add($"O E-mail deve conter no mínimo {email.MinLength} caracteres.");
                }
            }
            #endregion

            #region PhoneNumber

            var phoneNumber = PhoneNumberValueObject.Create(request.PhoneNumber!);
            if (!phoneNumber.IsNullOrEmpty) 
            {
                if (phoneNumber.IsGreaterThanMaxLength)
                {
                    errors.Add($"O telefone deve conter no máximo {phoneNumber.MaxLength} dígitos.");
                }
                if (phoneNumber.IsLessThanMinLength)
                {
                    errors.Add($"O telefone deve conter no mínimo {phoneNumber.MinLength} dígitos.");
                }
                if (!phoneNumber.IsDigitOnly)
                {
                    errors.Add("O telefone deve conter apenas números.");
                }
            }

            #endregion

            #region DocumentNumber

            var documentNumber = DocumentNumberValueObject.Create(request.DocumentNumber!);
            if (documentNumber.IsNullOrWhiteSpace)
            {
                errors.Add("O CNPJ/CPF deve ser informado.");
            }

            if (!documentNumber.IsNullOrWhiteSpace)
            {
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
                if (documentNumber.IsDigitOnly && !documentNumber.IsGreaterThanMaxLength && !documentNumber.IsLessThanMinLength)
                {
                    if (!documentNumber.IsValid)
                    {
                        errors.Add("O CNPJ/CPF informado é inválido.");
                    }
                }
            }

            #endregion

            return Task.FromResult(new RequestValidationModel(errors));
        }
    }
}
