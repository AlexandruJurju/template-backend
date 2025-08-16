using System.Collections.Immutable;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Common.SharedKernel.Application.CQRS.Behaviors;
using Template.Common.SharedKernel.Domain;

namespace Template.Common.SharedKernel.Application.CQRS.Mediator;

public static class Extensions
{
    public static void AddDefaultMediatR<TMarker>(
        this IServiceCollection services,
        Action<MediatRServiceConfiguration>? options = null
    )
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(TMarker).Assembly);

            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(QueryCachingBehavior<,>));
            options?.Invoke(config);
        });
    }

    public static async Task DispatchDomainEventsAsync(this IPublisher publisher, DbContext ctx)
    {
        var domainEntities = ctx
            .ChangeTracker.Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Count != 0)
            .ToImmutableList();

        var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToImmutableList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }
}
