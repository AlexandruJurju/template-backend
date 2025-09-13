using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Template.Common.SharedKernel.Domain;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework.Repository;

public static class QuerySpecEvaluator
{
    public static IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, QuerySpec<TEntity>? spec)
        where TEntity : Entity
    {
        if (spec == null)
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

        if (spec.Filter != null)
        {
            query = query.Where(spec.Filter);
        }

        foreach (Expression<Func<TEntity, object?>> include in spec.Includes)
        {
            query = query.Include(include);
        }

        if (spec.OrderBys.Any())
        {
            (Expression<Func<TEntity, object>> KeySelector, ListSortDirection Direction) firstOrderBy = spec.OrderBys.First();
            query = firstOrderBy.Direction == ListSortDirection.Descending
                ? query.OrderByDescending(firstOrderBy.KeySelector)
                : query.OrderBy(firstOrderBy.KeySelector);

            foreach ((Expression<Func<TEntity, object>> KeySelector, ListSortDirection Direction) orderBy in spec.OrderBys.Skip(1))
            {
                var orderedQuery = (IOrderedQueryable<TEntity>)query;
                query = orderBy.Direction == ListSortDirection.Descending
                    ? orderedQuery.ThenByDescending(orderBy.KeySelector)
                    : orderedQuery.ThenBy(orderBy.KeySelector);
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

    public static IQueryable<TResult> Apply<TEntity, TResult>(
        IQueryable<TEntity> query,
        QuerySpec<TEntity>? spec, Func<IQueryable<TEntity>, IQueryable<TResult>> projectionFunc
    ) where TEntity : Entity
    {
        var baseQuery = Apply(query, spec);
        return projectionFunc(baseQuery);
    }
}
