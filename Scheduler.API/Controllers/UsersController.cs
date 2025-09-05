using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Controllers.Base;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.Login.UseCase;
using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
using System;
using System.Threading.Tasks;

namespace Scheduler.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class UsersController(
        IUseCase<RegisterUserRequest, Response> registerUserUseCase,
        IUseCase<LoginRequest, Response> loginUseCase) : BaseController
    {   
        private readonly IUseCase<RegisterUserRequest, Response> _registerUserUseCase = registerUserUseCase;
        private readonly IUseCase<LoginRequest, Response> _loginUseCase = loginUseCase;

        [HttpGet]
        public async Task<IActionResult> ListUsersAsync([FromQuery] string? name = null, [FromQuery] string? documentNumber = null)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync([FromRoute] Guid id)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var result = await _loginUseCase.ExecuteAsync(request);
            return GetHttpResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserRequest request)
        {
            var result = await _registerUserUseCase.ExecuteAsync(request);
            return GetHttpResponse(result);
        }

        [HttpPatch("{id}")]
        public Task<IActionResult> UpdateUserAsync([FromRoute] Guid id, [FromBody] object company)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }
    }
}
