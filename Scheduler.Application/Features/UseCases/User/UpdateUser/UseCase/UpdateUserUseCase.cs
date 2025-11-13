using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.Cypher;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase.Interfaces;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Configuration;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.UpdateUser.UseCase
{
    internal sealed class UpdateUserUseCase(
        IUserRepository userRepository,
        IRequestValidator<UpdateUserRequest> validator,
        IFireBaseAuthenticationService authenticationService,
        IUserSession userSession) : IUseCase<UpdateUserRequest, Response>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRequestValidator<UpdateUserRequest> _validator = validator;
        private readonly IFireBaseAuthenticationService _authenticationService = authenticationService;
        private readonly IUserSession _userSession = userSession;

        private const string CypherAesKeyEnvironmentVariableName = "CYPHER_AES_KEY";

        public async Task<Response> ExecuteAsync(UpdateUserRequest request)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Response.CreateInvalidParametersResponse(validationResult.ErrorMessage);
                }

                var currentUser = await _userRepository.GetUserByIdAsync(request.Id);
                if (currentUser == null)
                {
                    return Response.CreateNotFoundResponse();
                }
                
                var isNotAdminUserSession = !_userSession.IsAdmin;
                var isTryingToUpdateAnotherUser = _userSession.UserExternalId != currentUser.ExternalId;
                var isTryingToChangeAdminStatus = request.IsAdmin.HasValue && request.IsAdmin.Value != currentUser.IsAdmin;
                if (isNotAdminUserSession && (isTryingToUpdateAnotherUser || isTryingToChangeAdminStatus))
                {                     
                    return Response.CreateForbiddenResponse();
                }

                var isTryingToChangeUserDocumentNumber = request.DocumentNumber != null && currentUser.DocumentNumber != request.DocumentNumber;
                if (isTryingToChangeUserDocumentNumber)
                {
                    var existsUserWithSameDocumentNumber = (await _userRepository.GetUserByDocumentNumberAsync(request.DocumentNumber!)) != null;
                    if (existsUserWithSameDocumentNumber)
                    {
                        return Response.CreateConflictResponse("Já existe um usuário cadastrado com esse número de documento.");
                    }
                }

                currentUser.Name = request.Name ?? currentUser.Name;
                currentUser.DocumentNumber = request.DocumentNumber ?? currentUser.DocumentNumber;
                currentUser.IsAdmin = request.IsAdmin ?? currentUser.IsAdmin;
                if (request.Password != null)
                {
                    var cypherAesKey = EnvironmentVariableHandler.GetEnvironmentVariable(CypherAesKeyEnvironmentVariableName);
                    var encryptedPassword = AES.Encrypt(request.Password, cypherAesKey);
                    currentUser.PasswordHash = encryptedPassword;
                }

                var (_, registeredWithSuccess) = await _authenticationService
                    .UpdateFireBaseUserAsync(currentUser.ExternalId, currentUser.Email, request.Password!, currentUser.Name, currentUser.IsAdmin);
                if (!registeredWithSuccess)
                {
                    return Response.CreateInternalErrorResponse("Não foi possível cadastrar o usuário.");
                }

                await _userRepository.UpdateUserAsync(currentUser.Id, currentUser);

                return Response.CreateOkResponse();
            }
            catch (Exception ex)
            {
                return Response.CreateInternalErrorResponse($"{nameof(UpdateUserUseCase)}->{nameof(ExecuteAsync)}: {ex.Message}");
            }
        }
    }
}
