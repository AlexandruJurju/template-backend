using Template.Application.Abstractions.Messaging;

namespace Template.Application.Abstractions.Cache;

public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;

public interface ICachedQuery
{
    string CacheKey { get; }

    TimeSpan Expiration { get; }
}
