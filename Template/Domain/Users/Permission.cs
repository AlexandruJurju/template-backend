namespace Domain.Users;

public sealed class Permission
{
    public static readonly Permission UsersRead = new(1, "users:read");
    public static readonly Permission UsersEdit = new(2, "users:edit");

    public Permission(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
}
