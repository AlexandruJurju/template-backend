using Template.Domain.Abstractions;
using Template.SharedKernel;

namespace Template.Domain.Users;

public sealed class User : Entity
{
    private User(Guid id, string email, string firstName, string lastName, string passwordHash)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
    }

    // For EF Core
    private User()
    {
    }

    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string PasswordHash { get; init; } = null!;
    public Role Role { get; private set; }

    public static User Create(string email, string firstName, string lastName, string passwordHash)
    {
        var user = new User(Guid.NewGuid(), email, firstName, lastName, passwordHash)
        {
            // user.Raise(new UserRegisteredDomainEvent(user.Id));
            Role = Role.Member
        };

        return user;
    }
}
