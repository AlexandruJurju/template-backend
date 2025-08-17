using Template.Common.SharedKernel.Infrastructure;
using Template.Common.SharedKernel.Infrastructure.Helpers;

namespace Template.Common.SharedKernel.Domain;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; init; } = DateTimeHelper.UtcNow();
    public DateTime? LastModifiedAt { get; set; }
    public Guid Version { get; set; }
}

public abstract class AuditableEntity<TId> : Entity<TId>
    where TId : IEquatable<TId>
{
    public DateTime CreatedAt { get; init; } = DateTimeHelper.UtcNow();
    public DateTime? LastModifiedAt { get; set; }
    public Guid Version { get; set; }
}
