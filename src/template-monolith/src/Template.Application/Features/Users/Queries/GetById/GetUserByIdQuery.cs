using Template.Application.Features.Users.Dto;
using Template.Common.SharedKernel.Application.CQRS.Queries;

namespace Template.Application.Features.Users.Queries.GetById;

public record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
