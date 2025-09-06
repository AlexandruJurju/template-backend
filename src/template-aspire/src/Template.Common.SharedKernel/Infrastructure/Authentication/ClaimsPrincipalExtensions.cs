using System.Security.Claims;
using Ardalis.GuardClauses;

namespace Template.Common.SharedKernel.Infrastructure.Authentication;

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
        var userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        Guard.Against.Null(userId, nameof(userId), "User ID was not found or is invalid");

        return Guid.Parse(userId);
    }
}
