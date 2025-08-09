using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Template.Application.Abstractions.Authentication;
using Template.Domain.Users;

namespace Template.Infrastructure.Authentication;

internal sealed class TokenProvider(
    IOptions<JwtOptions> jwtOptions
) : ITokenProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string GenerateToken(User user)
    {
        string secretKey = _jwtOptions.Secret;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role.Name),
                ..user.Role.Permissions.Select(p => new Claim("permission", p.Name))
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience
        };

        var handler = new JsonWebTokenHandler();

        string? token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}
