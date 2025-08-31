using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.RegisterUser.Validators
{
    internal sealed class RegisterUserValidator : IRequestValidator<RegisterUserRequest>
    {
        public Task<RequestValidationModel> ValidateAsync(RegisterUserRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
