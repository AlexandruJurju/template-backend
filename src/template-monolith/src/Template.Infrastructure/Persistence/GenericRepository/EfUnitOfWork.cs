
using Template.Common.SharedKernel.Infrastructure.Repository;

namespace Template.Infrastructure.Persistence.GenericRepository;

public class EfUnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
