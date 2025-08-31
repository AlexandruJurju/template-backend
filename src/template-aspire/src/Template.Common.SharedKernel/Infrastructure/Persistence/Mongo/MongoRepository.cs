using System.ComponentModel;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.Mongo;

public class MongoRepository<TDocument>(IMongoDatabase database) : IMongoRepository<TDocument>
    where TDocument : IDocument
{
    private readonly IMongoCollection<TDocument> _collection = database.GetCollection<TDocument>(typeof(TDocument).Name);

    public async Task<TDocument?> FirstOrDefaultAsync(Expression<Func<TDocument, bool>>? filter, CancellationToken cancellationToken = default)
    {
        IQueryable<TDocument>? queryable = _collection.AsQueryable();
        return await queryable.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<TDocument?> FirstOrDefaultAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            return default;
        }

        FilterDefinition<TDocument>? filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TDocument>> ListAsync(MongoQuerySpec<TDocument>? spec = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TDocument> queryable = Apply(spec);
        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<long> CountAsync(Expression<Func<TDocument, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        if (filter == null)
        {
            return await _collection.CountDocumentsAsync(FilterDefinition<TDocument>.Empty, cancellationToken: cancellationToken);
        }

        return await _collection.AsQueryable().LongCountAsync(filter, cancellationToken);
    }

    public async Task AddAsync(TDocument document, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default)
    {
        var documentList = documents.ToList();
        if (!documentList.Any())
        {
            return;
        }

        await _collection.InsertManyAsync(documentList, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(TDocument document, CancellationToken cancellationToken = default)
    {
        FilterDefinition<TDocument>? filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        await _collection.ReplaceOneAsync(filter, document, cancellationToken: cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default)
    {
        var documentList = documents.ToList();
        if (!documentList.Any())
        {
            return;
        }

        IEnumerable<WriteModel<TDocument>> writeModels = documentList.Select(doc =>
            new ReplaceOneModel<TDocument>(
                Builders<TDocument>.Filter.Eq(d => d.Id, doc.Id),
                doc
            )
        );

        await _collection.BulkWriteAsync(writeModels, cancellationToken: cancellationToken);
    }

    public async Task<bool> RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            return false;
        }

        FilterDefinition<TDocument>? filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        DeleteResult? result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<long> RemoveAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default)
    {
        DeleteResult? result = await _collection.DeleteManyAsync(filter, cancellationToken);
        return result.DeletedCount;
    }

    public async Task ReplaceOneAsync(Expression<Func<TDocument, bool>> filter, TDocument document, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(filter, document, cancellationToken: cancellationToken);
    }

    private IQueryable<TDocument> Apply(MongoQuerySpec<TDocument>? spec)
    {
        IQueryable<TDocument> queryable = _collection.AsQueryable();

        if (spec?.Filter is not null)
        {
            queryable = queryable.Where(spec.Filter);
        }

        if (spec?.OrderBys is not null && spec.OrderBys.Count > 0)
        {
            // Apply first ordering
            (Expression<Func<TDocument, object>> keySelector, ListSortDirection direction) = spec.OrderBys[0];
            IOrderedQueryable<TDocument> orderedQueryable = direction == ListSortDirection.Descending
                ? queryable.OrderByDescending(keySelector)
                : queryable.OrderBy(keySelector);

            // Apply ThenBy clauses
            foreach ((Expression<Func<TDocument, object>> thenKeySelector, ListSortDirection thenDirection) in spec.OrderBys.Skip(1))
            {
                orderedQueryable = thenDirection == ListSortDirection.Descending
                    ? orderedQueryable.ThenByDescending(thenKeySelector)
                    : orderedQueryable.ThenBy(thenKeySelector);
            }

            queryable = orderedQueryable;
        }

        if (spec?.Skip is not null)
        {
            queryable = queryable.Skip(spec.Skip.Value);
        }

        if (spec?.Take is not null)
        {
            queryable = queryable.Take(spec.Take.Value);
        }

        return queryable;
    }
}
