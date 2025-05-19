using Application.Abstractions.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

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

        IApplicationDbContext dbContext = httpContext.RequestServices.GetService<IApplicationDbContext>()!;

        bool isValid = await dbContext.ApiKeys
            .AnyAsync(k => k.Id == apiKeyGuid && k.IsActive);

        return isValid;
    }
}
