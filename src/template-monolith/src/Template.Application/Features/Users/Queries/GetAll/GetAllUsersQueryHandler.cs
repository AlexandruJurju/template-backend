using Template.Application.Mappers;
using Template.Common.SharedKernel.Application.CQRS.Queries;
using Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;
using Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework.Repository;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Queries.GetAll;

internal sealed class GetAllUsersQueryHandler(
    IRepository<User> userRepository
) : IQueryHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    public async Task<Result<IEnumerable<UserResponse>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var spec = new QuerySpec<User, UserResponse>
        {
            Projection = u => u.Map(),
        };

        List<UserResponse> users = await userRepository.GetAllAsync(spec, cancellationToken);

        return users.ToList();
    }
}
