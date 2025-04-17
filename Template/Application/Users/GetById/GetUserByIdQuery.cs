using Application.Abstractions.Messaging;

namespace Application.Users.GetById;

public record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
