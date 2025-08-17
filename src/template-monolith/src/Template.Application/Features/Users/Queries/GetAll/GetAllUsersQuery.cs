using Template.Application.Features.Users.Dto;
using Template.Common.SharedKernel.Application.CQRS.Queries;

namespace Template.Application.Features.Users.Queries.GetAll;

public record GetAllUsersQuery : IQuery<IEnumerable<UserResponse>>;
