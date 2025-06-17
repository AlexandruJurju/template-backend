using Application.Abstractions.Messaging;

namespace Application.Users.GetAll;

public record GetAllUsersQuery : IQuery<IEnumerable<UserResponse>>;
