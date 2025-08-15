namespace Template.Application.Features.Users.Queries.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
