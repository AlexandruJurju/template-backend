using MassTransit;

namespace Template.Common.SharedKernel.Application.EventBus;

public abstract class IntegrationEventHandler<TIntegrationEvent>
    : IIntegrationEventHandler<TIntegrationEvent>, IConsumer<TIntegrationEvent>
    where TIntegrationEvent : class, IIntegrationEvent
{
    public abstract Task Handle(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);

    public Task Handle(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) =>
        Handle((TIntegrationEvent)integrationEvent, cancellationToken);

    public Task Consume(ConsumeContext<TIntegrationEvent> context) =>
        Handle(context.Message, context.CancellationToken);
}
