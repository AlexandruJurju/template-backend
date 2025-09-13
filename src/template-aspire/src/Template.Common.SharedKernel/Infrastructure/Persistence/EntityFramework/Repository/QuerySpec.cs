using System.ComponentModel;
using System.Linq.Expressions;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework.Repository;

public interface IQuerySpec<TEntity>
{
    Expression<Func<TEntity, bool>>? Filter { get; }
    List<Expression<Func<TEntity, object?>>> Includes { get; }
    List<(Expression<Func<TEntity, object>> KeySelector, ListSortDirection Direction)> OrderBys { get; }
    int? Skip { get; }
    int? Take { get; }
    bool AsNoTracking { get; }
    bool AsSplitQuery { get; }
    bool IgnoreQueryFilters { get; }
}

public interface IQuerySpec<TEntity, TResult> : IQuerySpec<TEntity>
{
    Expression<Func<TEntity, TResult>>? Projection { get; }
}

public abstract class QuerySpecBase<TEntity>
{
    public Expression<Func<TEntity, bool>>? Filter { get; protected set; }
    public List<Expression<Func<TEntity, object?>>> Includes { get; protected set; } = new();
    public List<(Expression<Func<TEntity, object>> KeySelector, ListSortDirection Direction)> OrderBys { get; protected set; } = new();
    public int? Skip { get; protected set; }
    public int? Take { get; protected set; }
    public bool AsNoTracking { get; protected set; } = true;
    public bool AsSplitQuery { get; protected set; }
    public bool IgnoreQueryFilters { get; protected set; }

    protected void ApplyWhere(Expression<Func<TEntity, bool>> criteria)
    {
        Filter = Filter == null
            ? criteria
            : Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(Filter.Body, criteria.Body), Filter.Parameters);
    }

    protected void ApplyInclude(Expression<Func<TEntity, object?>> includeExpression) => Includes.Add(includeExpression);
    protected void ApplyOrderBy(Expression<Func<TEntity, object>> expr, ListSortDirection dir) => OrderBys.Add((expr, dir));

    protected void ApplyPaginate(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    protected void ApplyPage(int page, int size)
    {
        Skip = (page - 1) * size;
        Take = size;
    }

    protected void ApplyTracking(bool tracking) => AsNoTracking = !tracking;
    protected void ApplySplitQuery() => AsSplitQuery = true;
    protected void ApplyIgnoreQueryFilters() => IgnoreQueryFilters = true;
}

public abstract class QuerySpecBuilder<TEntity, TSpec> : QuerySpecBase<TEntity>, IQuerySpec<TEntity>
    where TSpec : QuerySpecBuilder<TEntity, TSpec>
{
    public TSpec Where(Expression<Func<TEntity, bool>> criteria)
    {
        ApplyWhere(criteria);
        return (TSpec)this;
    }

    public TSpec Include(Expression<Func<TEntity, object?>> expr)
    {
        ApplyInclude(expr);
        return (TSpec)this;
    }

    public TSpec OrderBy(Expression<Func<TEntity, object>> expr)
    {
        ApplyOrderBy(expr, ListSortDirection.Ascending);
        return (TSpec)this;
    }

    public TSpec OrderByDescending(Expression<Func<TEntity, object>> expr)
    {
        ApplyOrderBy(expr, ListSortDirection.Descending);
        return (TSpec)this;
    }

    public TSpec ThenBy(Expression<Func<TEntity, object>> expr)
    {
        ApplyOrderBy(expr, ListSortDirection.Ascending);
        return (TSpec)this;
    }

    public TSpec ThenByDescending(Expression<Func<TEntity, object>> expr)
    {
        ApplyOrderBy(expr, ListSortDirection.Descending);
        return (TSpec)this;
    }

    public TSpec Paginate(int skip, int take)
    {
        ApplyPaginate(skip, take);
        return (TSpec)this;
    }

    public TSpec Page(int page, int size)
    {
        ApplyPage(page, size);
        return (TSpec)this;
    }

    public TSpec WithTracking()
    {
        ApplyTracking(true);
        return (TSpec)this;
    }

    public TSpec WithNoTracking()
    {
        ApplyTracking(false);
        return (TSpec)this;
    }

    public TSpec WithSplitQuery()
    {
        ApplySplitQuery();
        return (TSpec)this;
    }

    public TSpec WithIgnoreQueryFilters()
    {
        ApplyIgnoreQueryFilters();
        return (TSpec)this;
    }
}

public class QuerySpec<TEntity> : QuerySpecBuilder<TEntity, QuerySpec<TEntity>>
{
    public static QuerySpec<TEntity> Create() => new();
}

public class QuerySpec<TEntity, TResult> : QuerySpecBuilder<TEntity, QuerySpec<TEntity, TResult>>, IQuerySpec<TEntity, TResult>
{
    public Expression<Func<TEntity, TResult>>? Projection { get; private set; }

    public static QuerySpec<TEntity, TResult> Create() => new();

    public QuerySpec<TEntity, TResult> Select(Expression<Func<TEntity, TResult>> selector)
    {
        Projection = selector;
        return this;
    }
}
