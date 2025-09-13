using System.Linq.Expressions;
using Template.Common.SharedKernel.Domain;
using Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework.Repository;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;

public interface IRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity>
    where TEntity : Entity;

public interface IReadRepository<TEntity>
    where TEntity : Entity
{
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, List<Expression<Func<TEntity, object?>>> includes,
        CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> projection, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAllAsync(IQuerySpec<TEntity> spec, CancellationToken cancellationToken = default);
    Task<List<TResult>> GetAllAsync<TResult>(IQuerySpec<TEntity> spec, Expression<Func<TEntity, TResult>> projection, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}

public interface IWriteRepository<TEntity>
    where TEntity : Entity
{
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    void Attach(TEntity entity);
}
