using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.Shared;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Repository;
using Scheduler.Application.Infrastructure.Data.Shared.Helpers.Pagination;
using System.Threading.Tasks;

namespace Scheduler.Application.Features.UseCases.User.ListUsers.UseCase
{
    internal sealed class ListUsersUseCase(IUserRepository userRepository, IUserSession userSession) : IUseCase<ListUsersRequest, Response>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUserSession _userSession = userSession;

        public async Task<Response> ExecuteAsync(ListUsersRequest input)
        {
            try
            {
                if (!_userSession.IsAdmin)
                {
                    return Response.CreateForbiddenResponse();
                }

                var users = await _userRepository.ListUsersAsync(input.Name, input.Email, input.DocumentNumber, input.IsAdmin, input.PaginationParameters);
                var getUserResponseList = users.Results.ConvertAll(user => new GetUserResponse(user));
                var result = new PaginatedQueryResult<GetUserResponse>(getUserResponseList, users.TotalCount);
                return Response.CreateOkResponse(result);
            }
            catch (System.Exception ex)
            {
                return Response.CreateInternalErrorResponse($"{nameof(ListUsersUseCase)}->{nameof(ExecuteAsync)}: {ex.Message}");
            }
        }
    }
}
