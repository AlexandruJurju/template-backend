using Template.SharedKernel.Application.Messaging;

namespace Template.Application.Users.Queries.GetById;

public record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
