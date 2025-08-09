using Template.SharedKernel.Domain;

namespace Template.Infrastructure.Data.GenericRepository;

public class EfUnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
