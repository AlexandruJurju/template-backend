using Ardalis.Result;

namespace Template.Domain.EmailTemplates;

public static class EmailTemplateErrors
{
    // 404
    public static Result NotFound(string templateName) =>
        Result.NotFound($"EmailTemplates.NotFound: The user with the Id: '{templateName}' was not found");
}
