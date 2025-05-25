using Domain.ApiKeys;

namespace Domain.Abstractions.Persistence;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> IsKeyValidAsync(Guid id, CancellationToken cancellationToken = default);
}
