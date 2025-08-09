namespace Template.Domain.EmailTemplates;

public static class EmailTemplateErrors
{
    public static Result NotFound(string templateName)
    {
        return Result.NotFound($"EmailTemplates.NotFound: The user with the Id: '{templateName}' was not found");
    }
}
