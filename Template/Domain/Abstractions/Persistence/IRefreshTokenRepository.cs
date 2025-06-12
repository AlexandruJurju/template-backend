using Domain.Users;

namespace Domain.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);

    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}
