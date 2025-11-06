using Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Features.UseCases.User.Shared
{
    [ExcludeFromCodeCoverage]
    internal sealed class GetUserResponse(UserEntity entity)
    {
        public Guid Id { get; private set; } = entity.Id;
        public string Name { get; private set; } = entity.Name;
        public string DocumentNumber { get; private set; } = entity.DocumentNumber;
        public string Email { get; private set; } = entity.Email;
        public string ExternalId { get; private set; } = entity.ExternalId;
        public bool IsAdmin { get; private set; } = entity.IsAdmin;
        public Guid? CompanyId { get; private set; } = entity.CompanyId;
        public DateTime CreatedAt { get; private set; } = entity.CreatedAt;
    }
}