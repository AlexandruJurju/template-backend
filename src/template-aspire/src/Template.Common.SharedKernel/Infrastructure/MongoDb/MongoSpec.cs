using System.ComponentModel;
using System.Linq.Expressions;

namespace Template.Common.SharedKernel.Infrastructure.MongoDb;

public sealed record MongoQuerySpec<T>(
    Expression<Func<T, bool>>? Filter = null,
    List<(Expression<Func<T, object>> KeySelector, ListSortDirection Direction)>? OrderBys = null,
    int? Skip = null,
    int? Take = null
) where T : IDocument;
