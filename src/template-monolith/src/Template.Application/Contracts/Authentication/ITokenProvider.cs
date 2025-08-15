using Template.Domain.Entities.Users;

namespace Template.Application.Contracts.Authentication;

public interface ITokenProvider
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
