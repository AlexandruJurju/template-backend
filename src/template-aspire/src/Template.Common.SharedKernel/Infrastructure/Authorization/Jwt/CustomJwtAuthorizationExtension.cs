using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Template.Common.SharedKernel.Infrastructure.Authorization.Jwt;

public static class CustomJwtAuthorizationExtension
{
    public static void AddDefaultJwtAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }
}
