namespace Domain.Users;

public sealed class Role(int id, string name)
{
    public static readonly Role Member = new(1, "Member");
    public static readonly Role Manager = new(2, "Manager");
    public static readonly Role Admin = new(3, "Admin");

    public int Id { get; init; } = id;
    public string Name { get; init; } = name;

    public ICollection<User> Users { get; init; } = new List<User>();
    public ICollection<Permission> Permissions { get; init; } = new List<Permission>();
}
