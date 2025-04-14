using Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

internal sealed class PermissionProvider(IApplicationDbContext context)
{
    public async Task<HashSet<string>> GetForUserIdAsync(Guid userId)
    {
        // todo: cache
        var permissions = await context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Role!.Permissions)
            .ToListAsync();

        var permissionsSet = permissions.Select(p => p.Name).ToHashSet();
        
        return permissionsSet;

        // return
        // [
        //     ..await context.Users
        //         .Where(u => u.Id == userId)
        //         .SelectMany(u => u.Role!.Permissions.Select(p => p.Name))
        //         .ToListAsync()
        // ];
    }
}