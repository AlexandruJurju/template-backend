using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
