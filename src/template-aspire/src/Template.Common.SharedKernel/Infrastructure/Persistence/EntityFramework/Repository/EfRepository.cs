using System.ComponentModel;
using System.Linq.Expressions;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Template.Common.SharedKernel.Domain;
using Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework.Repository;

public class EfRepository<TEntity>(DbContext dbContext) : IRepository<TEntity>
    where TEntity : Entity
{
    private DbSet<TEntity> DbSet { get; } = dbContext.Set<TEntity>();

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<List<TEntity>> GetAllAsync(
        IQuerySpec<TEntity>? spec = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpec(DbSet.AsQueryable(), spec);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<TResult>> GetAllAsync<TResult>(
        IQuerySpec<TEntity, TResult>? spec = null,
        CancellationToken cancellationToken = default)
    {
        // todo: fix
        Guard.Against.Null(spec?.Projection, nameof(spec.Projection));

        IQueryable<TEntity> query = ApplySpec(DbSet.AsQueryable(), spec);
        return await query.Select(spec.Projection).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        DbSet.UpdateRange(entities);
    }

    public void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }


    private static IQueryable<TEntity> ApplySpec(
        IQueryable<TEntity> query,
        IQuerySpec<TEntity>? spec)
    {
        if (spec is null)
        {
            return query.AsNoTracking();
        }

        if (spec.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (spec.IgnoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (spec.AsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        if (spec.Filter is not null)
        {
            query = query.Where(spec.Filter);
        }

        foreach (Expression<Func<TEntity, object?>> include in spec.Includes)
        {
            query = query.Include(include);
        }

        if (spec.OrderBys.Any())
        {
            (Expression<Func<TEntity, object>> KeySelector, ListSortDirection Direction) first = spec.OrderBys.First();
            query = first.Direction == ListSortDirection.Descending
                ? query.OrderByDescending(first.KeySelector)
                : query.OrderBy(first.KeySelector);

            foreach ((Expression<Func<TEntity, object>> KeySelector, ListSortDirection Direction) orderBy in spec.OrderBys.Skip(1))
            {
                var ordered = (IOrderedQueryable<TEntity>)query;
                query = orderBy.Direction == ListSortDirection.Descending
                    ? ordered.ThenByDescending(orderBy.KeySelector)
                    : ordered.ThenBy(orderBy.KeySelector);
            }
        }

        if (spec.Skip.HasValue)
        {
            query = query.Skip(spec.Skip.Value);
        }

        if (spec.Take.HasValue)
        {
            query = query.Take(spec.Take.Value);
        }

        return query;
    }
}
