using Scheduler.Application.Features.Shared.IO;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.UpdateUser.UseCase
{
    [ExcludeFromCodeCoverage]
    public sealed class UpdateUserRequest : IRequest
    {
        public string? Name { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Password { get; set; }
        public bool? IsAdmin { get; set; }

        public Guid Id { get; private set; }

        public void SetId(Guid id)
        {
            Id = id;
        }
    }
}
