namespace Template.Domain.Entities.Users;

public sealed class User : Entity
{
    // For EF Core
    private User()
    {
    }

    private User(Guid id, string email, string firstName, string lastName, string passwordHash)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
    }

    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string PasswordHash { get; init; } = null!;
    public int RoleId { get; init; }
    public Role Role { get; private set; }

    public static User Create(string email, string firstName, string lastName, string passwordHash)
    {
        var user = new User(Guid.NewGuid(), email, firstName, lastName, passwordHash)
        {
            RoleId = Role.Member.Id
        };

        user.RegisterDomainEvent(new UserRegisteredDomainEvent(user.Id));

        return user;
    }
}
