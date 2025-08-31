using System.ComponentModel;
using System.Linq.Expressions;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.Mongo;

public interface IMongoRepository<TDocument> where TDocument : IDocument
{
    Task<TDocument?> FirstOrDefaultAsync(Expression<Func<TDocument, bool>>? filter, CancellationToken cancellationToken = default);
    Task<TDocument?> FirstOrDefaultAsync(string id, CancellationToken cancellationToken = default);
    Task<List<TDocument>> ListAsync(MongoQuerySpec<TDocument>? spec = null, CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<TDocument, bool>>? filter = null, CancellationToken cancellationToken = default);
    Task AddAsync(TDocument document, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default);
    Task UpdateAsync(TDocument document, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(string id, CancellationToken cancellationToken = default);
    Task<long> RemoveAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default);
    Task ReplaceOneAsync(Expression<Func<TDocument, bool>> filter, TDocument document, CancellationToken cancellationToken = default);
}

public sealed record MongoQuerySpec<T>(
    Expression<Func<T, bool>>? Filter = null,
    List<(Expression<Func<T, object>> KeySelector, ListSortDirection Direction)>? OrderBys = null,
    int? Skip = null,
    int? Take = null
) where T : IDocument;
