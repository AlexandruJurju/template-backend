using System.ComponentModel.DataAnnotations.Schema;

namespace Template.SharedKernel.Domain;

public class HasDomainEventsBase : IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = new();

    [NotMapped] public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
