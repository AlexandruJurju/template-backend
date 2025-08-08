namespace Template.SharedKernel.Domain;

public abstract class EntityBase : HasDomainEventsBase
{
    public Guid Id { get; set; }
}

public abstract class EntityBase<TId> : HasDomainEventsBase
    where TId : struct, IEquatable<TId>
{
    public TId Id { get; set; } = default!;
}

/// <summary>
/// For use with Vogen or similar tools for generating code for
/// strongly typed Ids.
/// </summary>
public abstract class EntityBase<T, TId> : HasDomainEventsBase
    where T : EntityBase<T, TId>
{
    public TId Id { get; set; } = default!;
}
