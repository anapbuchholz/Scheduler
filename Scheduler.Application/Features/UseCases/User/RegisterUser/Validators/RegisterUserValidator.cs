using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.Shared.ValueObjects;
using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.RegisterUser.Validators
{
    internal sealed class RegisterUserValidator : IRequestValidator<RegisterUserRequest>
    {
        private readonly int _nameMaxLength = 255;
        private readonly int _passwordMinLength = 6;
        private readonly int _passwordMaxLength = 16;

        public Task<RequestValidationModel> ValidateAsync(RegisterUserRequest request)
        {
            var errors = new List<string>();

            #region Name
            var isNameEmpty = string.IsNullOrWhiteSpace(request.Name);
            if (isNameEmpty)
            {
                errors.Add("O nome deve ser informado.");
            }
            if (!isNameEmpty && request.Name?.Length > _nameMaxLength)
            {
                errors.Add($"O nome deve conter no máximo {_nameMaxLength} caracteres.");
            }
            #endregion

            #region Password
            var isPasswordEmpty = string.IsNullOrWhiteSpace(request.Password);
            if (isPasswordEmpty)
            {
                errors.Add("A senha deve ser informada.");
            }
            if (!isPasswordEmpty && request.Password!.Length < _passwordMinLength)
            {
                errors.Add($"A senha deve conter no mínimo {_passwordMinLength} caracteres.");
            }
            if (!isPasswordEmpty && request.Password!.Length > _passwordMaxLength)
            {
                errors.Add($"A senha deve conter no máximo {_passwordMaxLength} caracteres.");
            }
            #endregion

            #region DocumentNumber
            var documentNumber = DocumentNumberValueObject.Create(request.DocumentNumber!);
            if (documentNumber.IsNullOrWhiteSpace)
            {
                errors.Add("O número do CPF deve ser informado.");
            }
            
            if (!documentNumber.IsNullOrWhiteSpace)
            {
                if (!documentNumber.IsDigitOnly)
                {
                    errors.Add("O número do CPF deve conter apenas números.");
                }
                if (documentNumber.IsGreaterThanMaxLength)
                {
                    errors.Add($"O número do CPF deve conter no máximo {documentNumber.MaxLength} dígitos.");
                }
                if (documentNumber.IsLessThanMinLength)
                {
                    errors.Add($"O número do CPF deve conter no mínimo {documentNumber.MinLength} dígitos.");
                }
                if (documentNumber.IsDigitOnly && !documentNumber.IsGreaterThanMaxLength && !documentNumber.IsLessThanMinLength)
                {
                    if (!documentNumber.IsValid)
                    {
                        errors.Add("O número do CPF informado é inválido.");
                    }
                }
            }
            #endregion

            #region Email
            var email = EmailValueObject.Create(request.Email!);
            if (email.IsNullOrWhiteSpace)
            {
                errors.Add("O E-mail deve ser informado.");
            }
            if (!email.IsNullOrWhiteSpace)
            {
                if (email.IsGreaterThanMaxLength)
                {
                    errors.Add($"O E-mail deve conter no máximo {email.MaxLength} caracteres.");
                }
                if (email.IsLessThanMinLength)
                {
                    errors.Add($"O E-mail deve conter no mínimo {email.MinLength} caracteres.");
                }
                if (!email.IsValidEmail)
                {
                    errors.Add("O E-mail informado é inválido.");
                }
            }

            #endregion

            #region CompanyId
            if (!request.IsAdmin && request.CompanyId == null)
            {
                errors.Add("O Id da empresa deve ser informado para usuários não administradores.");
            }
            #endregion

            return Task.FromResult(new RequestValidationModel(errors));
        }
    }
}
