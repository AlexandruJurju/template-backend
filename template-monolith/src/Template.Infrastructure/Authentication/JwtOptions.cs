namespace Template.Infrastructure.Authentication;

public class JwtOptions
{
    public string Secret { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public int ExpireInMinutes { get; init; }
    public int RefreshTokenExpireInMinutes { get; init; }
}
