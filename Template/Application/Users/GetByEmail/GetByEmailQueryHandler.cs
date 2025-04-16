using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.GetByEmail;

public class GetByEmailQueryHandler(IApplicationDbContext context)
{
    public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        UserResponse? user = await context.Users
            .Where(u => u.Email == query.Email)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? Result.Failure<UserResponse>(UserErrors.NotFound(query.Email))
            : Result.Success(user);
    }
}
