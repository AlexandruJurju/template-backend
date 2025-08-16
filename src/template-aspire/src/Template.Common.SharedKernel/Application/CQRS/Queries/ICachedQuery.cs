namespace Template.Common.SharedKernel.Application.CQRS.Queries;

public interface ICachedQuery
{
    string CacheKey { get; }

    TimeSpan Expiration { get; }
}

public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;
