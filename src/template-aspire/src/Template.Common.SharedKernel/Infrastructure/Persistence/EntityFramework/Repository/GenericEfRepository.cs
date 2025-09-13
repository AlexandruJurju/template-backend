using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Template.Common.SharedKernel.Domain;
using Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework.Repository;

// note - i hate generic repository, i'm just testing how it behaves with microservices and DDD
public class GenericEfRepository<TEntity>(DbContext dbContext) : IRepository<TEntity>
    where TEntity : Entity
{
    private DbSet<TEntity> DbSet { get; } = dbContext.Set<TEntity>();

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        List<Expression<Func<TEntity, object?>>> includes,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = DbSet.AsQueryable();

        foreach (Expression<Func<TEntity, object?>> include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<List<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> projection, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Select(projection).ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetAllAsync(IQuerySpec<TEntity> spec, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = ApplySpec(DbSet.AsQueryable(), spec);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<TResult>> GetAllAsync<TResult>(IQuerySpec<TEntity> spec, Expression<Func<TEntity, TResult>> projection,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = ApplySpec(DbSet.AsQueryable(), spec);
        return await query.Select(projection).ToListAsync(cancellationToken);
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

    public void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbSet.AddRange(entities);
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

    public void Attach(TEntity entity)
    {
        throw new NotImplementedException();
    }


    private static IQueryable<TEntity> ApplySpec(
        IQueryable<TEntity> query,
        IQuerySpec<TEntity> spec)
    {
        if (spec.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (spec.IgnoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (spec.IgnoreAutoIncludes)
        {
            query = query.IgnoreAutoIncludes();
        }

        if (spec.AsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        if (spec.Where is not null)
        {
            query = query.Where(spec.Where);
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

        query = query.Skip(spec.Skip);

        query = query.Take(spec.Take);

        return query;
    }
}
