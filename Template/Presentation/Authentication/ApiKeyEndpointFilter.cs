using Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Presentation.Authentication;

public sealed class ApiKeyEndpointFilter : IEndpointFilter
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
        IApplicationDbContext dbContext = context.HttpContext.RequestServices.GetRequiredService<IApplicationDbContext>();

        bool isValid = await dbContext.ApiKeys
            .AnyAsync(k => k.Id == apiKeyGuid && k.IsActive, context.HttpContext.RequestAborted);

        if (!isValid)
        {
            return TypedResults.Unauthorized();
        }

        // 4. Proceed to endpoint if valid
        return await next(context);
    }
}
