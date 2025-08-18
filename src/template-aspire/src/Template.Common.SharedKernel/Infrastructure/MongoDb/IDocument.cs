using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Template.Common.SharedKernel.Infrastructure.Helpers;

namespace Template.Common.SharedKernel.Infrastructure.MongoDb;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    ObjectId Id { get; init; }

    DateTime CreatedAt { get; init; }
}
