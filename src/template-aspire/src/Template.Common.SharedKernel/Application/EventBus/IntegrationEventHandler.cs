using MassTransit;

namespace Template.Common.SharedKernel.Application.EventBus;

public abstract class IntegrationEventHandler<TIntegrationEvent>
    : IIntegrationEventHandler<TIntegrationEvent>, IConsumer<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    public abstract Task Handle(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);

    public Task Handle(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        return Handle((TIntegrationEvent)integrationEvent, cancellationToken);
    }

    public Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        return Handle(context.Message, context.CancellationToken);
    }
}
