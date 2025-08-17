using MassTransit;

namespace Template.Common.SharedKernel.Application.EventBus;

public class EventBus(
    IBus bus
) : IEventBus
{
    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent
    {
        await bus.Publish(integrationEvent, cancellationToken);
    }
}
