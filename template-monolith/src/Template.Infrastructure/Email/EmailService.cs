using FluentEmail.Core;
using FluentEmail.Core.Models;
using Template.Application.Abstractions.Email;
using Template.Domain.Users;

namespace Template.Infrastructure.Email;

public class EmailService(IFluentEmail fluentEmail) : IEmailService
{
    public async Task<Result> SendEmail<TModel>(string toMail, string subject, string emailTemplateName, TModel model)
    {
        SendResponse? result = await fluentEmail
            .To(toMail)
            .Subject(subject)
            .UsingTemplateFromFile(emailTemplateName, model, isHtml: true)
            .SendAsync();

        return !result.Successful
            ? UserErrors.EmailNotSent()
            : Result.Success();
    }
}
