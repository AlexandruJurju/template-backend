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
        QuerySpec<User, UserResponse> spec = QuerySpec<User, UserResponse>
            .Create()
            .OrderBy(x => x.FirstName)
            .Select(x => x.Map());

        IReadOnlyList<UserResponse> users = await userRepository.GetAllAsync(spec, cancellationToken);

        return users.ToList();
    }
}
