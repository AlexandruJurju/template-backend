using Domain.Users;
using Mediator;

namespace Application.Users.Register;

public sealed class UserRegisteredDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    public ValueTask Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
