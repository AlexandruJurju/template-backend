namespace Template.Application.Features.Users.Queries.GetById;

public record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
