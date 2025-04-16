using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.LoggedInUser;

public class LoggedInUserQueryHandler(IUserContext userContext, IApplicationDbContext context)
{
    public async Task<Result<User>> Handle(LoggedInUserQuery query, CancellationToken cancellationToken)
    {
        User? user = await context.Users
            .SingleOrDefaultAsync(cancellationToken);

        return user ?? Result.Failure<User>(UserErrors.NotFound(userContext.UserId));
    }
}
