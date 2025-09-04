using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Validation;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.Login.UseCase
{
    internal sealed class LoginUseCase(IRequestValidator<LoginRequest> validator) : IUseCase<LoginRequest, Response>
    {
        private readonly IRequestValidator<LoginRequest> _validator = validator;

        public async Task<Response> ExecuteAsync(LoginRequest input)
        {
            var validationResult = await _validator.ValidateAsync(input);
            if (!validationResult.IsValid)
            {
                return Response.CreateInvalidParametersResponse(validationResult.ErrorMessage);
            }

            //TODO: Implementar lógica de login aqui.
        }
    }
}
