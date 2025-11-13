using Scheduler.Application.Features.Shared.IO;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.RegisterUser.UseCase
{
    [ExcludeFromCodeCoverage]
    public sealed class RegisterUserRequest : IRequest
    {
        public string? Name { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsAdmin { get; set; } = false;
        public Guid? CompanyId { get; set; }
    }
}
