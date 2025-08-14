using Template.Application.Mapping;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Queries.GetByEmail;

internal sealed class GetUserByEmailQueryHandler(
    IApplicationDbContext dbContext
) : IQueryHandler<GetUserByEmailQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .Where(x => x.Email == query.Email)
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound(query.Email);
        }

        return user.Map();
    }
}
