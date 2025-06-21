﻿using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace Infrastructure.Authorization;

internal sealed class PermissionProvider(
    ApplicationDbContext context,
    HybridCache hybridCache
)
{
    public async Task<HashSet<string>> GetForUserIdAsync(Guid userId)
    {
        HashSet<string> cachedPermissions = await hybridCache.GetOrCreateAsync(
            $"auth:roles:{userId}",
            async cancellationToken =>
            {
                // Database fetch if not in cache
                List<string> permissions = await context.Users
                    .Where(u => u.Id == userId)
                    .SelectMany(u => u.Role!.Permissions)
                    .Select(p => p.Name)
                    .ToListAsync(cancellationToken);

                return permissions.ToHashSet();
            }
        );

        return cachedPermissions;
    }
}
