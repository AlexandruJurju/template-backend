using Microsoft.EntityFrameworkCore;
using Template.Common.SharedKernel.Domain;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework;

public static class QueryableExtensions
{
    public static IQueryable<T> ForTenant<T>(this DbSet<T> dbSet, Guid tenantId)
        where T : class, ITenantOwned
    {
        return dbSet.Where(t => t.TenantId == tenantId);
    }
}
