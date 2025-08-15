using MediatR;

namespace Template.Common.SharedKernel.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn => DateTime.UtcNow;
}
