using Template.Domain.Abstractions;
using Template.SharedKernel;

namespace Template.Domain.EmailTemplates;

public class EmailTemplate : Entity
{
    public static readonly string UserRegistered = "UserRegistered";

    public string Name { get; init; }
    public string Subject { get; init; }
    public string Content { get; init; }
}
