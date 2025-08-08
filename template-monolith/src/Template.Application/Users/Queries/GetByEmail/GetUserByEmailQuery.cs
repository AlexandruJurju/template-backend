using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.Queries.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
