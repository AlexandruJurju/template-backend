using Microsoft.AspNetCore.Http;

namespace Template.Common.SharedKernel.Infrastructure.Authentication.Jwt;

internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid GetUserId()
    {
        return httpContextAccessor
                   .HttpContext?
                   .User
                   .GetUserId()
               ?? throw new ApplicationException("User context is unavailable");
    }
}
