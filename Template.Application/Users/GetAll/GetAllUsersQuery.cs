using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.GetAll;

public record GetAllUsersQuery : IQuery<IEnumerable<UserResponse>>;
