using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.Login.UseCase
{
    internal sealed class LoginUseCase(
        IRequestValidator<LoginRequest> validator,
        IFireBaseAuthenticationService fireBaseAuthenticationService) : IUseCase<LoginRequest, Response>
    {
        private readonly IRequestValidator<LoginRequest> _validator = validator;
        private readonly IFireBaseAuthenticationService _fireBaseAuthenticationService = fireBaseAuthenticationService;

        public async Task<Response> ExecuteAsync(LoginRequest request)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Response.CreateInvalidParametersResponse(validationResult.ErrorMessage);
                }

                var (IsAuthenticated, JwtToken) = await _fireBaseAuthenticationService.LoginInFireBase(request.Email!, request.Password!);
                if (!IsAuthenticated)
                {
                    return Response.CreateNotFoundResponse("Nome de usuário ou senha não encontrados.");
                }

                return Response.CreateOkResponse(new { Token = JwtToken });
            }
            catch (System.Exception ex)
            {
                return Response.CreateInternalErrorResponse($"{nameof(LoginUseCase)}->{nameof(ExecuteAsync)}: {ex.Message}");
            }
        }
    }
}
