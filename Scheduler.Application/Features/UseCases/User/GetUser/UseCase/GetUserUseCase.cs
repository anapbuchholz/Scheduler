using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.Shared;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using System;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.GetUser.UseCase
{
    internal sealed class GetUserUseCase(IUserRepository userRepository, IUserSession userSession) : IUseCase<GetUserRequest, Response>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUserSession _userSession = userSession;

        public async Task<Response> ExecuteAsync(GetUserRequest input)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(input.UserId);

                if (user == null)
                {
                    return Response.CreateNotFoundResponse();
                }

                if (!_userSession.IsAdmin && user.ExternalId != _userSession.UserId)
                {
                    return Response.CreateForbiddenResponse();
                }

                var result = new GetUserResponse(user);

                return Response.CreateOkResponse(result);
            }
            catch (Exception ex)
            {
                return Response.CreateInternalErrorResponse($"{nameof(GetUserUseCase)}->{nameof(ExecuteAsync)}: {ex.Message}");
            }
        }
    }
}
