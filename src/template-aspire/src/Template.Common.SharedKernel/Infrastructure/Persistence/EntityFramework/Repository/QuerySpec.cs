using System.ComponentModel;
using System.Linq.Expressions;
using Template.Common.SharedKernel.Domain;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework.Repository;

public interface IQuerySpec<TEntity> where TEntity : Entity
{
    Expression<Func<TEntity, bool>>? Where { get; set; }
    List<Expression<Func<TEntity, object?>>> Includes { get; set; }
    List<(Expression<Func<TEntity, object>> KeySelector, ListSortDirection Direction)> OrderBys { get; set; }
    int Skip { get; set; }
    int Take { get; set; }
    bool AsNoTracking { get; set; }
    bool AsSplitQuery { get; set; }
    bool IgnoreQueryFilters { get; set; }
    bool IgnoreAutoIncludes { get; set; }
}

public class QuerySpec<TEntity> : IQuerySpec<TEntity> where TEntity : Entity
{
    public Expression<Func<TEntity, bool>>? Where { get; set; }
    public List<Expression<Func<TEntity, object?>>> Includes { get; set; } = new();
    public List<(Expression<Func<TEntity, object>> KeySelector, ListSortDirection Direction)> OrderBys { get; set; } = new();
    public int Skip { get; set; }
    public int Take { get; set; } = 20;
    public bool AsNoTracking { get; set; } = true;
    public bool AsSplitQuery { get; set; }
    public bool IgnoreQueryFilters { get; set; }
    public bool IgnoreAutoIncludes { get; set; }
}
