using Microsoft.AspNetCore.Authorization;

namespace Template.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User is not { Identity.IsAuthenticated: true })
        {
            return Task.CompletedTask;
        }

        // If token contains a json array then the permissions will be interpreted as separate claims
        // Use a HashSet to store unique permissions
        var permissions = context.User
            .FindAll("permission")
            .Select(c => c.Value)
            .ToHashSet();

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
