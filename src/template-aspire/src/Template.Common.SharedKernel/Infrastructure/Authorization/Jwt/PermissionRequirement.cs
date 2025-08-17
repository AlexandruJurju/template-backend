using Microsoft.AspNetCore.Authorization;

namespace Template.Common.SharedKernel.Infrastructure.Authorization.Jwt;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
