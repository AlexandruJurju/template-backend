using Template.Common.SharedKernel.Infrastructure.Helpers;

namespace Template.Common.SharedKernel.Application.EventBus;

public abstract record IntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime CreationDate { get; } = DateTimeHelper.UtcNow();
}
