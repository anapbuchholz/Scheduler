using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.Shared.ValueObjects;
using Scheduler.Application.Features.UseCases.User.UpdateUser.UseCase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.UpdateUser.Validators
{
    internal sealed class UpdateUserValidator : IRequestValidator<UpdateUserRequest>
    {
        public Task<RequestValidationModel> ValidateAsync(UpdateUserRequest request)
        {
            var errors = new List<string>();

            #region Name

            if (request.Name != null)
            {
                var userNameValueObject = UserNameValueObject.Create(request.Name);
                if (userNameValueObject.IsNullOrWhiteSpace)
                {
                    errors.Add("O nome informado é inválido.");
                }
                if (userNameValueObject.IsGreaterThanMaxLength)
                {
                    errors.Add($"O nome deve conter no máximo {userNameValueObject.MaxLength} caracteres.");
                }
            }

            #endregion

            #region Document Number

            if (request.DocumentNumber != null)
            {
                var documentNumberValueObject = DocumentNumberValueObject.Create(request.DocumentNumber!);

                if (!documentNumberValueObject.IsDigitOnly)
                {
                    errors.Add("O número do documento deve conter apenas dígitos numéricos.");
                }
                if (documentNumberValueObject.IsGreaterThanMaxLength)
                {
                    errors.Add($"O número do documento deve conter no máximo {documentNumberValueObject.MaxLength} dígitos.");
                }
                if (documentNumberValueObject.IsLessThanMinLength)
                {
                    errors.Add($"O número do documento deve conter no mínimo {documentNumberValueObject.MinLength} dígitos.");
                }
                if (documentNumberValueObject.IsDigitOnly && !documentNumberValueObject.IsGreaterThanMaxLength && !documentNumberValueObject.IsLessThanMinLength)
                {
                    if (!documentNumberValueObject.IsValid)
                    {
                        errors.Add("O número do documento informado é inválido.");
                    }
                }
            }

            #endregion

            #region Password

            if (request.Password != null)
            {
                var userPasswordValueObject = UserPasswordValueObject.Create(request.Password!);
                if (userPasswordValueObject.IsNullOrEmpty)
                {
                    errors.Add("A senha informada é inválida.");
                }
                if (userPasswordValueObject.IsLessThanMinLength)
                {
                    errors.Add($"A senha deve conter no mínimo {userPasswordValueObject.MinLength} caracteres.");
                }
                if (userPasswordValueObject.IsGreaterThanMaxLength)
                {
                    errors.Add($"A senha deve conter no máximo {userPasswordValueObject.MaxLength} caracteres.");
                }
            }

            #endregion

            return Task.FromResult(new RequestValidationModel(errors));
        }
    }
}
