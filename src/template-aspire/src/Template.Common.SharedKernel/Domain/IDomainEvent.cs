using MediatR;
using Template.Common.SharedKernel.Infrastructure;

namespace Template.Common.SharedKernel.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUtc => DateTimeHelper.UtcNow();
}
