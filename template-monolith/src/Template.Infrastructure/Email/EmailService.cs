using FluentEmail.Core;
using FluentEmail.Core.Models;
using Template.Application.Abstractions.Email;
using Template.Domain.EmailTemplates;
using Template.Domain.Users;

namespace Template.Infrastructure.Email;

public class EmailService(IFluentEmail fluentEmail) : IEmailService
{
    public async Task<Result> SendEmail<TModel>(string toMail, EmailTemplate emailTemplate, TModel model)
    {
        SendResponse? result = await fluentEmail
            .To(toMail)
            .Subject(emailTemplate.Subject)
            .UsingTemplate(emailTemplate.Content, model)
            .SendAsync();

        return !result.Successful
            ? UserErrors.EmailNotSent()
            : Result.Success();
    }
}
