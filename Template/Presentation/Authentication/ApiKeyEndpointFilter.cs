using Domain.Abstractions.Persistence;

namespace Presentation.Authentication;

public sealed class ApiKeyEndpointFilter(
    IApiKeyRepository apiKeyRepository
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
        bool isValid = await apiKeyRepository.IsKeyValidAsync(apiKeyGuid);

        if (!isValid)
        {
            return TypedResults.Unauthorized();
        }

        // 4. Proceed to endpoint if valid
        return await next(context);
    }
}
