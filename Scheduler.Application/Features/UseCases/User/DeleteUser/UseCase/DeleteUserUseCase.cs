using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.DeleteUser.UseCase
{
    internal sealed class DeleteUserUseCase(
        IUserRepository userRepository,
        IFireBaseAuthenticationService authenticationService,
        IUserSession userSession) : IUseCase<DeleteUserRequest, Response>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IFireBaseAuthenticationService _authenticationService = authenticationService;
        private readonly IUserSession _userSession = userSession;

        public async Task<Response> ExecuteAsync(DeleteUserRequest input)
        {
            try
            {
                if (!_userSession.IsAdmin)
                {
                    return Response.CreateNotFoundResponse();
                }

                var user = await _userRepository.GetUserByIdAsync(input.Id);
                if (user == null)
                {
                    return Response.CreateNotFoundResponse();
                }

                var (_, deletedWithSucces) = await _authenticationService.DeleteFireBaseUserAsync(user.Email);

                if (!deletedWithSucces)
                {
                    return Response.CreateInternalErrorResponse("Não foi possível excluir o usuário");
                }

                await _userRepository.DeleteUserAsync(input.Id);

                return Response.CreateOkResponse();
            }
            catch (Exception ex)
            {
                return Response.CreateInternalErrorResponse($"{nameof(DeleteUserUseCase)}->{nameof(ExecuteAsync)}: {ex.Message}");
            }
        }
    }
}
