using Scheduler.Application.Features.Shared.IO;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.GetUser.UseCase
{
    [ExcludeFromCodeCoverage]
    public sealed class GetUserRequest : IRequest
    {
        public required Guid UserId { get; init; }
    }
}
