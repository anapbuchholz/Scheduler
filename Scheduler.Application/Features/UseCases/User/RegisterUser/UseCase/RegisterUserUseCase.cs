using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.Cypher;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Infrastructure.Authentication.Services.FireBase;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Configuration;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Repository;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase
{
    internal sealed class RegisterUserUseCase(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IRequestValidator<RegisterUserRequest> validator,
        IFireBaseAuthenticationService authenticationService,
        IUserSession userSession) : IUseCase<RegisterUserRequest, Response>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICompanyRepository _companyRepository = companyRepository;
        private readonly IRequestValidator<RegisterUserRequest> _validator = validator;
        private readonly IFireBaseAuthenticationService _authenticationService = authenticationService;
        private readonly IUserSession _userSession = userSession;

        private const string CypherAesKeyEnvironmentVariableName = "CYPHER_AES_KEY";

        public async Task<Response> ExecuteAsync(RegisterUserRequest input)
        {
            try
            {
                //TODO: OBTER DADOS DO USUÁRIO LOGADO ATRAVÉS DO TOKEN E VALIDAR SE ELE É ADMINISTRADOR.
                //SE NÃO FOR ADMINISTRADOR, SÓ PODE CADASTRAR USUÁRIO QUE SEJA DA MESMA EMPRESA.
                //SE FOR ADMINISTRADOR PODE CADASTRAR USUÁRIO DE QUALQUER EMPRESA.
                //APENAS USUÁRIOS ADMINISTRADORES PODEM CADASTRAR OUTROS USUÁRIOS ADMINISTRADORES.

                var validationResult = await _validator.ValidateAsync(input);
                if (!validationResult.IsValid)
                {
                    return Response.CreateInvalidParametersResponse(validationResult.ErrorMessage);
                }

                var existingUser = await _userRepository.GetUserByEmailAsync(input.Email!);
                if (existingUser != null)
                {
                    return Response.CreateConflictResponse("Já existe um usuário cadastrado com esse email.");
                }

                if(!input.IsAdmin)
                {
                    var usersCompany = await _companyRepository.GetCompanyAsync(input.CompanyId!.Value);
                    if (usersCompany != null)
                    {
                        return Response.CreateInvalidParametersResponse("A empresa informada não existe.");
                    }
                }

                var userPermission = input.IsAdmin ? "1" : "0";
                var (ExternalId, RegisteredWithSuccess) = await _authenticationService.RegisterFireBaseUserAsync(input.Email!, input.Password!, $"{input.Name}-{userPermission}");
                if (!RegisteredWithSuccess)
                {
                    return Response.CreateInternalErrorResponse("Não foi possível cadastrar o usuário.");
                }

                var AesKey = EnrionmentVariableHandler.GetEnvironmentVariable(CypherAesKeyEnvironmentVariableName);
                var passwordHash = AES.Encrypt(input.Password!, AesKey);
                var userEntity = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    ExternalId = ExternalId!,
                    CreatedAt = DateTime.UtcNow,
                    IsAdmin = input.IsAdmin,
                    Name = input.Name!,
                    Email = input.Email!,
                    CompanyId = input.IsAdmin ? null : input.CompanyId!.Value,
                    DocumentNumber = input.DocumentNumber!,
                    PasswordHash = passwordHash
                };
                await _userRepository.RegisterUserAsync(userEntity);

                return Response.CreateCreatedResponse();
            }
            catch (Exception ex)
            {
                return Response.CreateInternalErrorResponse($"{nameof(RegisterUserUseCase)}->{nameof(ExecuteAsync)}: {ex.Message}");
            }
        }
    }
}
