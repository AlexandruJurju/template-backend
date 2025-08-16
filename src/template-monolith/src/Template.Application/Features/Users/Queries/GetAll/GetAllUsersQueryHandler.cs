using Template.Common.SharedKernel.Application.CQRS.Queries;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Queries.GetAll;

public class GetAllUsersQueryHandler(
    IApplicationDbContext dbContext
) : IQueryHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    public async Task<Result<IEnumerable<UserResponse>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<User> users = await dbContext.Users.ToListAsync(cancellationToken);

        var userResponses = users.Select(u => new UserResponse
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email
        }).ToList();

        return userResponses;
    }
}
