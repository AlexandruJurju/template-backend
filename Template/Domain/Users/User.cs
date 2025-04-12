using Domain.Abstractions;

namespace Domain.Users;

public sealed class User : Entity
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string PasswordHash { get; init; } = null!;
}