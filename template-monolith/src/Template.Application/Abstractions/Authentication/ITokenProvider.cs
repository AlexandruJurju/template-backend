using Template.Domain.Users;

namespace Template.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
