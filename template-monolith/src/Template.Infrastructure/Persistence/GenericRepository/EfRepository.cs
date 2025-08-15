using System.ComponentModel;
using System.Linq.Expressions;
using Template.SharedKernel.Domain;
using Template.SharedKernel.Infrastructure.Persistence;

namespace Template.Infrastructure.Persistence.GenericRepository;

public class EfRepository<TEntity>(ApplicationDbContext dbContext) : IRepository<TEntity> where TEntity : EntityBase
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public async Task<TEntity?> FirstOrDefaultAsync(QuerySpec<TEntity> spec, CancellationToken ct = default)
    {
        return await Apply(spec).FirstOrDefaultAsync(spec.Filter ?? (_ => true), ct);
    }

    public async Task<List<TEntity>> ListAsync(QuerySpec<TEntity>? spec = null, CancellationToken ct = default)
    {
        return await Apply(spec).ToListAsync(ct);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken ct = default)
    {
        return await _dbSet.CountAsync(filter ?? (_ => true), ct);
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        await _dbSet.AddRangeAsync(entities, ct);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    private IQueryable<TEntity> Apply(QuerySpec<TEntity>? spec)
    {
        IQueryable<TEntity> q = _dbSet;

        if (spec?.AsNoTracking ?? true)
        {
            q = q.AsNoTracking();
        }

        if (spec?.Filter is not null)
        {
            q = q.Where(spec.Filter);
        }

        if (spec?.Include is not null)
        {
            foreach (Expression<Func<TEntity, object?>> include in spec.Include)
            {
                q = q.Include(include);
            }

            q = q.AsSplitQuery();
        }

        if (spec?.OrderBys is not null && spec.OrderBys.Count > 0)
        {
            // Apply first ordering
            (Expression<Func<TEntity, object>> KeySelector, ListSortDirection Direction) first = spec.OrderBys[0];
            q = first.Direction == ListSortDirection.Descending
                ? q.OrderByDescending(first.KeySelector)
                : q.OrderBy(first.KeySelector);

            // Apply ThenBy clauses
            foreach ((Expression<Func<TEntity, object>> KeySelector, ListSortDirection Direction) ob in spec.OrderBys.Skip(1))
            {
                var ordered = (IOrderedQueryable<TEntity>)q;
                q = ob.Direction == ListSortDirection.Descending
                    ? ordered.ThenByDescending(ob.KeySelector)
                    : ordered.ThenBy(ob.KeySelector);
            }
        }


        if (spec?.Skip is not null)
        {
            q = q.Skip(spec.Skip.Value);
        }

        if (spec?.Take is not null)
        {
            q = q.Take(spec.Take.Value);
        }

        return q;
    }
}
