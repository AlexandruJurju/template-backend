using Domain.Abstractions;
using Mediator;

namespace Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
