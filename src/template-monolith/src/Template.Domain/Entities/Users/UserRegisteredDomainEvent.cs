namespace Template.Domain.Entities.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
