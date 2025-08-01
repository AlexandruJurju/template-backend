﻿using Template.Domain.Abstractions;

namespace Template.Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
