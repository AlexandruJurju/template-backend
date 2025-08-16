using System.Security.Claims;

namespace Template.Common.SharedKernel.Infrastructure.Auth;

public static class ClaimsPrincipalExtensions
{
    public static string[] GetRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return [.. claimsPrincipal.FindAll(ClaimTypes.Role).Select(c => c.Value)];
    }

    public static string? GetClaimValue(this ClaimsPrincipal claimsPrincipal, string claimType)
    {
        Claim? claim = claimsPrincipal.FindFirst(claimType);
        return claim?.Value;
    }

    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        // todo: improve
        return Guid.TryParse(userId, out Guid parsedUserId)
            ? parsedUserId
            : throw new ApplicationException("User id is unavailable");
    }
}
