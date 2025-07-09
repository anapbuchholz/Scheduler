using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduler.Infrastructure.Models
{
    public class Company
    {
        public Guid Id { get; set; }
        public string TradeName { get; set; }
        public string LegalName { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
