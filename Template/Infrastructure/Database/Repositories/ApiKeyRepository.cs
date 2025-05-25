using Domain.Abstractions.Persistence;
using Domain.ApiKeys;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class ApiKeyRepository(
    ApplicationDbContext dbContext
) : IApiKeyRepository
{
    public async Task<ApiKey?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.ApiKeys
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> IsKeyValidAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.ApiKeys
            .AnyAsync(e => e.Id == id && e.IsActive, cancellationToken);
    }
}
