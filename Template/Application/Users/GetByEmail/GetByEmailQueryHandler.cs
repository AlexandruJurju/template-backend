using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.GetByEmail;

public class GetByEmailQueryHandler
{
    private readonly IApplicationDbContext _context;

    public GetByEmailQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        var user = await _context.Users
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

        if (user is null)
            return Result.Failure<UserResponse>(UserErrors.NotFound(query.Email));

        return Result.Success(user);
    }
}