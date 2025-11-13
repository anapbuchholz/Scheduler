using Scheduler.Application.Features.Shared.IO;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.DeleteUser.UseCase
{
    [ExcludeFromCodeCoverage]
    public sealed class DeleteUserRequest : IRequest
    {
        public Guid Id { get; set; }
    }
}
