using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.LoggedInUser;

public class LoggedInUserQueryHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public LoggedInUserQueryHandler(IUserContext userContext, IApplicationDbContext context)
    {
        _userContext = userContext;
        _context = context;
    }

    public async Task<Result<User>> Handle(LoggedInUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
            return Result.Failure<User>(UserErrors.NotFound(_userContext.UserId));

        return user;
    }
}