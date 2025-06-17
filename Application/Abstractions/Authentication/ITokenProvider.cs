using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
