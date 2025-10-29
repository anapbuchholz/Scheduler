using Scheduler.Application.Infrastructure.Data.Shared.Entity;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity
{
    [ExcludeFromCodeCoverage]
    internal sealed class UserEntity : BaseEntity
    {
        public string Name { get; set; } = "INVALID REGISTER";
        public string DocumentNumber { get; set; } = "INVALID REGISTER";
        public string Email { get; set; } = "INVALID REGISTER";
        public string PasswordHash { get; set; } = "INVALID REGISTER";
        public string ExternalId { get; set; } = "INVALID REGISTER";
        public bool IsAdmin { get; set; } = false;
        public Guid? CompanyId { get; set; }
    }
}
