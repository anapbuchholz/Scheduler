using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.Cypher;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.Shared.IO.Validation;
using Scheduler.Application.Infrastructure.Authentication;
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
        IAuthenticationService authenticationService) : IUseCase<RegisterUserRequest, Response>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICompanyRepository _companyRepository = companyRepository;
        private readonly IRequestValidator<RegisterUserRequest> _validator = validator;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        private const string CypherAesKeyEnvironmentVariableName = "CYPHER_AES_KEY";

        public async Task<Response> ExecuteAsync(RegisterUserRequest input)
        {
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

            var usersCompany = await _companyRepository.GetCompanyAsync(input.CompanyId!.Value);
            if (usersCompany != null)
            {
                return Response.CreateInvalidParametersResponse("A empresa informada não existe.");
            }

            var AesKey = EnrionmentVariableHandler.GetEnvironmentVariable(CypherAesKeyEnvironmentVariableName);
            var passwordHash = AES.Encrypt(input.Password!, AesKey);

            //TODO: IMPLEMENTAR INTEGRAÇÃO COM FIREBASE PARA CADASTRAR USUÁRIO
            //var userPermission = input.IsAdmin ? 1 : 0;
            //var registeredInFirebaseWithSuccess = await _authenticationService.RegisterAsync(input.Email!, input.Password!, userPermission);
            //if(!registeredInFirebaseWithSuccess) throw...

            var userEntity = new UserEntity
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsAdmin = input.IsAdmin,
                Name = input.Name!,
                Email = input.Email!, 
                CompanyId = input.CompanyId!.Value,
                DocumentNumber = input.DocumentNumber!,
                PasswordHash = passwordHash
            };
            await _userRepository.RegisterUserAsync(userEntity);

            return Response.CreateCreatedResponse();
        }
    }
}
