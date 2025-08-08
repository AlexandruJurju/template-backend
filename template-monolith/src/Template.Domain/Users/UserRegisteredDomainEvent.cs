using Template.SharedKernel.Domain;

namespace Template.Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
