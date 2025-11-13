using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Controllers.Base;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.DeleteUser.UseCase;
using Scheduler.Application.Features.UseCases.User.GetUser.UseCase;
using Scheduler.Application.Features.UseCases.User.ListUsers.UseCase;
using Scheduler.Application.Features.UseCases.User.Login.UseCase;
using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
using Scheduler.Application.Features.UseCases.User.UpdateUser.UseCase;
using System;
using System.Threading.Tasks;

namespace Scheduler.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class UsersController(
        IUseCase<RegisterUserRequest, Response> registerUserUseCase,
        IUseCase<LoginRequest, Response> loginUseCase,
        IUseCase<GetUserRequest, Response> getUserUseCase,
        IUseCase<ListUsersRequest, Response> listUsersUseCase,
        IUseCase<UpdateUserRequest, Response> updateUserUseCase,
        IUseCase<DeleteUserRequest, Response> deleteUserUseCase) : BaseController
    {   
        private readonly IUseCase<RegisterUserRequest, Response> _registerUserUseCase = registerUserUseCase;
        private readonly IUseCase<LoginRequest, Response> _loginUseCase = loginUseCase;
        private readonly IUseCase<GetUserRequest, Response> _getUserUseCase = getUserUseCase;
        private readonly IUseCase<ListUsersRequest, Response> _listUsersUseCase = listUsersUseCase;
        private readonly IUseCase<UpdateUserRequest, Response> _updateUserUseCase = updateUserUseCase;
        private readonly IUseCase<DeleteUserRequest, Response> _deleteUserUseCase = deleteUserUseCase;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListUsersAsync([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? name = null, [FromQuery] string? documentNumber = null, [FromQuery] string? email = null, [FromQuery] bool? isAdmin = null)
        {
            var result = await _listUsersUseCase.ExecuteAsync(new ListUsersRequest(name, email, documentNumber, isAdmin, pageNumber, pageSize));
            return GetHttpResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync([FromRoute] Guid id)
        {
            var result = await _getUserUseCase.ExecuteAsync(new GetUserRequest { UserId = id });
            return GetHttpResponse(result);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var result = await _loginUseCase.ExecuteAsync(request);
            return GetHttpResponse(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserRequest request)
        {
            var result = await _registerUserUseCase.ExecuteAsync(request);
            return GetHttpResponse(result);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid id, [FromBody] UpdateUserRequest request)
        {
            request.SetId(id);
            var result = await _updateUserUseCase.ExecuteAsync(request);
            return GetHttpResponse(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
        {
            var result = await _deleteUserUseCase.ExecuteAsync(new DeleteUserRequest{ Id = id });
            return GetHttpResponse(result);
        }
    }
}
