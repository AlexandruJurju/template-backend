﻿using Template.Domain.Abstractions.Result;

namespace Template.Domain.EmailTemplates;

public static class EmailTemplateErrors
{
    public static Error NotFound(string templateName)
    {
        return Error.Problem(
            "EmailTemplates.NotFound",
            $"Email template with name {templateName} not found"
        );
    }
}
