using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Template.Common.SharedKernel.Domain;

public class HasDomainEventsBase
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [NotMapped] [JsonIgnore] public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

    protected void RegisterDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
