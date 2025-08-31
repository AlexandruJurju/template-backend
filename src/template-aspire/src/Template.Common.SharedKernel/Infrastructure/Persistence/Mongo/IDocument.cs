using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.Mongo;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    ObjectId Id { get; init; }

    DateTime CreatedAt { get; init; }
}
