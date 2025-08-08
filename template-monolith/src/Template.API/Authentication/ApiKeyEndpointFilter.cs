using Microsoft.EntityFrameworkCore;
using Template.Domain.Abstractions.Persistence;

namespace Template.API.Authentication;

public sealed class ApiKeyEndpointFilter(
    IApplicationDbContext dbContext
) : IEndpointFilter
{
    private const string ApiKeyHeaderName = "X-ApiKey";

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // 1. Extract API Key
        string? apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];

        // 2. Validate GUID Format
        if (!Guid.TryParse(apiKey, out Guid apiKeyGuid))
        {
            return TypedResults.Unauthorized();
        }

        // 3. Database Validation
        bool isValid = await dbContext.ApiKeys
            .AnyAsync(e => e.Id == apiKeyGuid && e.IsActive);

        if (!isValid)
        {
            return TypedResults.Unauthorized();
        }

        // 4. Proceed to endpoint if valid
        return await next(context);
    }
}
