using Template.SharedKernel.Domain;

namespace Template.SharedKernel.Infrastructure;

public interface IDomainEventsDispatcher
{
    Task DispatchAndClearEvents(IEnumerable<IHasDomainEvents> entitiesWithEvents);
}
