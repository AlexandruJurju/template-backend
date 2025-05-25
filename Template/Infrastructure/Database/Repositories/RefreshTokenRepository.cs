using Domain.Abstractions.Persistence;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class RefreshTokenRepository(
    ApplicationDbContext dbContext
) : IRefreshTokenRepository
{
    public void Add(RefreshToken refreshToken)
    {
        dbContext.RefreshTokens.Add(refreshToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await dbContext.RefreshTokens
            .SingleOrDefaultAsync(e => e.Token == token, cancellationToken);
    }
}
