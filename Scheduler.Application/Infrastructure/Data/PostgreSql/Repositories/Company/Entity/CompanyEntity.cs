using Scheduler.Application.Infrastructure.Data.Shared.Entity;

namespace Scheduler.Application.Infrastructure.Data.PostgreSql.Repositories.Company.Entity
{
    internal class CompanyEntity : BaseEntity
    {
        public string TradeName { get; set; }
        public string LegalName { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
    }
}
