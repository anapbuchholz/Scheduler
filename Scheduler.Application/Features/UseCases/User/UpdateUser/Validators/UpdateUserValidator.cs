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
            var userNameValueObject = UserNameValueObject.Create(request.Name!);
            if (userNameValueObject.IsGreaterThanMaxLength)
            {
                errors.Add($"O nome deve conter no máximo {userNameValueObject.MaxLength} caracteres.");
            }
            #endregion

            return Task.FromResult(new RequestValidationModel(errors));
        }
    }
}
