using Domain.Abstractions;

namespace Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;