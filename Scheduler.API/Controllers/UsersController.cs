using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Controllers.Base;
using Scheduler.Application.Features.Shared;
using Scheduler.Application.Features.Shared.IO;
using Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase;
using System;
using System.Threading.Tasks;

namespace Scheduler.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class UsersController(
        IUseCase<RegisterUserRequest, Response> registerUserUseCase) : BaseController
    {   
        private readonly IUseCase<RegisterUserRequest, Response> _registerUserUseCase = registerUserUseCase;

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

        [HttpPost]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserRequest input)
        {
            var result = await _registerUserUseCase.ExecuteAsync(input);
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
