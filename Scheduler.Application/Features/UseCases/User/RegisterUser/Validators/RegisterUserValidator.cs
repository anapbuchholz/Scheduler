using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.Shared.ValueObjects;
using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.RegisterUser.Validators
{
    internal sealed class RegisterUserValidator : IRequestValidator<RegisterUserRequest>
    {
        public Task<RequestValidationModel> ValidateAsync(RegisterUserRequest request)
        {
            var errors = new List<string>();

            #region Name

            var userNameValueObject = UserNameValueObject.Create(request.Name!);
            if (userNameValueObject.IsNullOrWhiteSpace)
            {
                errors.Add("O nome deve ser informado");
            }
            if (userNameValueObject.IsGreaterThanMaxLength)
            {
                errors.Add($"O nome deve conter no máximo {userNameValueObject.MaxLength} caracteres");
            }

            #endregion

            #region Password

            var userPasswordValueObject = UserPasswordValueObject.Create(request.Password!);
            if (userPasswordValueObject.IsNullOrEmpty)
            {
                errors.Add("A senha deve ser informada");
            }
            if (userPasswordValueObject.IsLessThanMinLength)
            {
                errors.Add($"A senha deve conter no mínimo {userPasswordValueObject.MinLength} caracteres");
            }
            if (userPasswordValueObject.IsGreaterThanMaxLength)
            {
                errors.Add($"A senha deve conter no máximo {userPasswordValueObject.MaxLength} caracteres");
            }

            #endregion

            #region DocumentNumber

            var documentNumber = DocumentNumberValueObject.Create(request.DocumentNumber!);
            if (documentNumber.IsNullOrWhiteSpace)
            {
                errors.Add("O número do documento deve ser informado");
            }
            
            if (!documentNumber.IsNullOrWhiteSpace)
            {
                if (!documentNumber.IsDigitOnly)
                {
                    errors.Add("O número do documento deve conter apenas dígitos numéricos");
                }
                if (documentNumber.IsGreaterThanMaxLength)
                {
                    errors.Add($"O número do documento deve conter no máximo {documentNumber.MaxLength} dígitos");
                }
                if (documentNumber.IsLessThanMinLength)
                {
                    errors.Add($"O número do documento deve conter no mínimo {documentNumber.MinLength} dígitos");
                }
                if (documentNumber.IsDigitOnly && !documentNumber.IsGreaterThanMaxLength && !documentNumber.IsLessThanMinLength)
                {
                    if (!documentNumber.IsValid)
                    {
                        errors.Add("O número do documento informado é inválido");
                    }
                }
            }

            #endregion

            #region Email

            var email = EmailValueObject.Create(request.Email!);
            if (email.IsNullOrWhiteSpace)
            {
                errors.Add("O E-mail deve ser informado");
            }
            if (!email.IsNullOrWhiteSpace)
            {
                if (email.IsGreaterThanMaxLength)
                {
                    errors.Add($"O E-mail deve conter no máximo {email.MaxLength} caracteres");
                }
                if (email.IsLessThanMinLength)
                {
                    errors.Add($"O E-mail deve conter no mínimo {email.MinLength} caracteres");
                }
                if (!email.IsValidEmail)
                {
                    errors.Add("O E-mail informado é inválido");
                }
            }

            #endregion

            #region CompanyId

            if (!request.IsAdmin && request.CompanyId == null)
            {
                errors.Add("O Id da empresa deve ser informado para usuários não administradores");
            }

            #endregion

            return Task.FromResult(new RequestValidationModel(errors));
        }
    }
}
