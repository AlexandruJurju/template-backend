using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.GetById;

public record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
