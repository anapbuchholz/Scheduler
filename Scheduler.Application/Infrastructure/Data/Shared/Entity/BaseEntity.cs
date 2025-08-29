using System;

namespace Scheduler.Application.Infrastructure.Data.Shared.Entity
{
    internal abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
