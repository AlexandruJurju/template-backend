using Microsoft.AspNetCore.Http;
using Template.Application.Abstractions.Authentication;

namespace Template.Infrastructure.Authentication;

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
