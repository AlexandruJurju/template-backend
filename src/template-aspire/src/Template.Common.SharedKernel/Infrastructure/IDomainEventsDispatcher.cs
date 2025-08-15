using Template.Common.SharedKernel.Domain;

namespace Template.Common.SharedKernel.Infrastructure;

public interface IDomainEventsDispatcher
{
    Task DispatchAndClearEvents(IEnumerable<IHasDomainEvents> entitiesWithEvents);
}
