namespace Template.Common.SharedKernel.Application.EventBus;

public abstract class IntegrationEvent(Guid id, DateTime occurredOnUtc) : IIntegrationEvent
{
    public Guid Id { get; init; } = id;

    public DateTime OccurredOnUtc { get; init; } = occurredOnUtc;
}
