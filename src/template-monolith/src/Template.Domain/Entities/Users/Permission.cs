namespace Template.Domain.Entities.Users;

public sealed class Permission(int id, string name)
{
    public static readonly Permission UsersRead = new(1, "users:read");
    public static readonly Permission UsersEdit = new(2, "users:edit");

    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
}
