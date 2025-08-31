using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Template.Common.SharedKernel.Domain;
using Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework.Repository;

public class EfRepository<TEntity>(DbContext dbContext) : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public async Task<TEntity?> FirstOrDefaultAsync(QuerySpec<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await Apply(spec).FirstOrDefaultAsync(spec.Filter ?? (_ => true), cancellationToken);
    }

    public async Task<List<TEntity>> ListAsync(QuerySpec<TEntity>? spec = null, CancellationToken cancellationToken = default)
    {
        return await Apply(spec).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(filter ?? (_ => true), cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
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
