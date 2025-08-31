using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using Template.Common.SharedKernel.Domain;
using Template.Common.SharedKernel.Infrastructure.Outbox;
using Template.Common.SharedKernel.Infrastructure.Serialization;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework;

public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            InsertOutboxMessages(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void InsertOutboxMessages(DbContext context)
    {
        var domainEvents = context
            .ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                IReadOnlyCollection<IDomainEvent> domainEvents = entity.DomainEvents;
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                domainEvent.OccurredOnUtc,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, SerializerSettings.Instance)
            ))
            .ToList();

        context.Set<OutboxMessage>().AddRange(outboxMessages);
    }
}
