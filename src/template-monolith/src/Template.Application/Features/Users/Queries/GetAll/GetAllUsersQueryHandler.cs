using Template.Application.Mappers;
using Template.Common.SharedKernel.Application.CQRS.Queries;
using Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;
using User = Template.Domain.Entities.Users.User;

namespace Template.Application.Features.Users.Queries.GetAll;

internal sealed class GetAllUsersQueryHandler(
    IRepository<User> userRepository
) : IQueryHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    public async Task<Result<IEnumerable<UserResponse>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        List<UserResponse> users = await userRepository.GetAllAsync(x => x.Map(), cancellationToken);

        return users;
    }
}
