using Scheduler.Application.Infrastructure.Data.Shared.Entity;
using System;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.User.Entity
{
    internal sealed class UserEntity : BaseEntity
    {
        public string Name { get; set; } = "INVALID REGISTER";
        public string DocumentNumber { get; set; } = "INVALID REGISTER";
        public string Email { get; set; } = "INVALID REGISTER";
        public string PasswordHash { get; set; } = "INVALID REGISTER";
        public string ExternalId { get; set; } = "INVALID REGISTER";
        public bool IsAdmin { get; set; }
        public Guid CompanyId { get; set; }
    }
}
