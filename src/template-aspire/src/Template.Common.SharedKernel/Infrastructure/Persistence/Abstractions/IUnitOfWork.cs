using System.Data.Common;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWork<out TContext> : IUnitOfWork
    where TContext : class
{
    TContext Context { get; }
}
