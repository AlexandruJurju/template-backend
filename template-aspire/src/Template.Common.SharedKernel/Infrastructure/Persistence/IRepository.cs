using System.Linq.Expressions;
using Template.Common.SharedKernel.Domain;

namespace Template.Common.SharedKernel.Infrastructure.Persistence;

public interface IRepository<T> where T : EntityBase
{
    Task<T?> FirstOrDefaultAsync(QuerySpec<T> spec, CancellationToken ct = default);
    Task<List<T>> ListAsync(QuerySpec<T>? spec = null, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
