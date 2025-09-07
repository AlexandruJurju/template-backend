using Template.Application.Features.Users.Dto;
using Template.Common.SharedKernel.Application.CQRS.Queries;
using Template.Common.SharedKernel.Application.Mapper;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Queries.GetAll;

internal sealed class GetAllUsersQueryHandler(
    IApplicationDbContext dbContext,
    IMapper<User, UserResponse> userMapper
) : IQueryHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    public async Task<Result<IEnumerable<UserResponse>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<User> users = await dbContext.Users.ToListAsync(cancellationToken);

        var userResponses = users.Select(userMapper.Map).ToList();

        return userResponses;
    }
}
