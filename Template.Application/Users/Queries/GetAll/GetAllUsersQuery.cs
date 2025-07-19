using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.Queries.GetAll;

public record GetAllUsersQuery : IQuery<IEnumerable<UserResponse>>;
