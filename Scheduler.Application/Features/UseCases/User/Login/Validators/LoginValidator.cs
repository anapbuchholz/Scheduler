using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.User.Login.UseCase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.Login.Validators
{
    internal sealed class LoginValidator : IRequestValidator<LoginRequest>
    {
        public Task<RequestValidationModel> ValidateAsync(LoginRequest request)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors.Add("O E-mail deve ser informado");
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors.Add("A senha deve ser informada");
            }

            return Task.FromResult(new RequestValidationModel(errors));
        }
    }
}
