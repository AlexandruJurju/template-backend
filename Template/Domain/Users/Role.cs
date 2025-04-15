namespace Domain.Users;

public sealed class Role
{
    public static readonly Role Member = new(1, "Member");
    public static readonly Role Manager = new(2, "Manager");
    public static readonly Role Admin = new(3, "Admin");

    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }
    public string Name { get; init; }

    public ICollection<User> Users { get; init; } = new List<User>();
    public ICollection<Permission> Permissions { get; init; } = new List<Permission>();
}
