namespace Template.Domain.EmailTemplates;

public class EmailTemplate : EntityBase
{
    public static readonly string UserRegistered = "UserRegistered";

    public string Name { get; init; }
    public string Subject { get; init; }
    public string Content { get; init; }
}
