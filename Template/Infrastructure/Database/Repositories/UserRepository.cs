using Domain.Abstractions.Persistence;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

internal sealed class UserRepository(
    ApplicationDbContext dbContext
) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Where(e => e.Id == id)
            .Include(e => e.Role)
            .ThenInclude(e => e.Permissions)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Where(e => e.Email == email)
            .Include(e => e.Role)
            .ThenInclude(e => e.Permissions)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UserWithEmailExists(string email, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.AnyAsync(e => e.Email == email, cancellationToken);
    }

    public void Add(User user)
    {
        dbContext.Attach(user.Role);

        dbContext.Users.Add(user);
    }
}
