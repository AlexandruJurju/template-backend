using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Template.Common.SharedKernel.Application.CQRS.Queries;

namespace Template.Common.SharedKernel.Application.CQRS.Behaviors;

public sealed class QueryCachingBehavior<TRequest, TResponse>(
    HybridCache hybridCache,
    ILogger<QueryCachingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        bool cacheMiss = false;

        TResponse result = await hybridCache.GetOrCreateAsync<TResponse>(
            request.CacheKey,
            async token =>
            {
                cacheMiss = true;
                logger.LogInformation("Cache miss for {Query}", typeof(TRequest).Name);
                return await next(token);
            },
            new HybridCacheEntryOptions
            {
                Expiration = request.Expiration,
                LocalCacheExpiration = request.Expiration
            },
            cancellationToken: cancellationToken
        );

        if (!cacheMiss)
        {
            logger.LogInformation("Cache hit for {Query}", typeof(TRequest).Name);
        }

        return result;
    }
}
