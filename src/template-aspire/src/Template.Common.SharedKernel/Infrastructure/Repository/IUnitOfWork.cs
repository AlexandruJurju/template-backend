
namespace Template.Common.SharedKernel.Infrastructure.Repository;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
