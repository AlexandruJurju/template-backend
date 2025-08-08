using Template.SharedKernel.Application.Messaging;

namespace Template.Application.Users.Queries.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
