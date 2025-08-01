﻿using Microsoft.AspNetCore.Authorization;

namespace Template.Infrastructure.Authorization;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
