using System.ComponentModel;
using System.Linq.Expressions;

namespace Template.Common.SharedKernel.Infrastructure.Repository;

public sealed record QuerySpec<T>(
    Expression<Func<T, bool>>? Filter = null,
    List<Expression<Func<T, object?>>>? Include = null,
    List<(Expression<Func<T, object>> KeySelector, ListSortDirection Direction)>? OrderBys = null,
    int? Skip = null,
    int? Take = null,
    bool AsNoTracking = true
);
