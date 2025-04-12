using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.GetAll;

public class GetAllUsersQueryHandler(IApplicationDbContext context)
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<List<UserResponse>>> Handle(GetAllUsersQuery usersQuery, CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new UserResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            })
            .ToListAsync(cancellationToken);

        return users;
    }
}