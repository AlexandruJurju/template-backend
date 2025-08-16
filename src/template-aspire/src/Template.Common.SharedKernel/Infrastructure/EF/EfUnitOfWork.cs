using Microsoft.EntityFrameworkCore;
using Template.Common.SharedKernel.Infrastructure.Repository;

namespace Template.Common.SharedKernel.Infrastructure.EF;

public class EfUnitOfWork<TDbContext>(TDbContext dbContext) : IUnitOfWork
    where TDbContext : DbContext
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
