using Template.Common.SharedKernel.Infrastructure.Helpers;

namespace Template.Common.SharedKernel.Application.EventBus;

public interface IIntegrationEvent
{
    Guid Id => Guid.NewGuid();

    DateTime OccurredOnUtc => DateTimeHelper.UtcNow();
}
