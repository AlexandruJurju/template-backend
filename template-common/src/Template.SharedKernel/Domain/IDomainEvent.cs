using MediatR;

namespace Template.SharedKernel.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn => DateTime.UtcNow;
}
