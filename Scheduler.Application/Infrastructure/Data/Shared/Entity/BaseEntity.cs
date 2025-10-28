using System;
using System.Diagnostics.CodeAnalysis;

namespace Scheduler.Application.Infrastructure.Data.Shared.Entity
{
    [ExcludeFromCodeCoverage]
    internal abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
