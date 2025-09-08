using Scheduler.Application.Features.Shared.IO;

namespace Scheduler.Application.Features.UseCases.User.Login.UseCase
{
    public sealed class LoginRequest : IRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
