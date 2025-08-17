using MediatR;
using Template.Common.SharedKernel.Infrastructure;
using Template.Common.SharedKernel.Infrastructure.Helpers;

namespace Template.Common.SharedKernel.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUtc => DateTimeHelper.UtcNow();
}
