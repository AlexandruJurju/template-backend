using Application.Abstractions.Email;
using Domain.Abstractions.Result;
using Domain.EmailTemplates;
using Domain.Users;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using RazorLight;

namespace Infrastructure.Email;

public class EmailService(IFluentEmail fluentEmail) : IEmailService
{
    public async Task<Result> SendEmail<TModel>(string toMail, EmailTemplate emailTemplate, TModel model)
    {
        SendResponse? result = await fluentEmail
            .To(toMail)
            .Subject(emailTemplate.Subject)
            .UsingTemplate(emailTemplate.Content, model, isHtml: true)
            .SendAsync();

        return !result.Successful
            ? Result.Failure(UserErrors.EmailNotSent())
            : Result.Success();
    }
}
