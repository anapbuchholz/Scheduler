using Scheduler.Application.Infrastructure.Data.Shared.Entity;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity
{
    [ExcludeFromCodeCoverage]
    internal sealed class CompanyEntity : BaseEntity
    {
        public string TradeName { get; set; } = "INVALID REGISTER";
        public string LegalName { get; set; } = "INVALID REGISTER";
        public string DocumentNumber { get; set; } = "INVALID REGISTER";
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
    }
}
