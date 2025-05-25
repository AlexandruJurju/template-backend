using Domain.Abstractions.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class ApiKeyAuthenticationAttribute : Attribute, IAsyncAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-ApiKey";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!await IsApiKeyValidAsync(context.HttpContext))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private async Task<bool> IsApiKeyValidAsync(HttpContext httpContext)
    {
        string? apiKey = httpContext.Request.Headers[ApiKeyHeaderName];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        if (!Guid.TryParse(apiKey, out Guid apiKeyGuid))
        {
            return false;
        }

        IApiKeyRepository apiKeyRepository = httpContext.RequestServices.GetService<IApiKeyRepository>()!;

        bool isValid = await apiKeyRepository.IsKeyValidAsync(apiKeyGuid);

        return isValid;
    }
}
