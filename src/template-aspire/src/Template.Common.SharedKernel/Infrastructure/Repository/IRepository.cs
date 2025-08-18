using System.Linq.Expressions;
using Template.Common.SharedKernel.Domain;

namespace Template.Common.SharedKernel.Infrastructure.Repository;

public interface IRepository<T> where T : Entity
{
    Task<T?> FirstOrDefaultAsync(QuerySpec<T> spec, CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(QuerySpec<T>? spec = null, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
