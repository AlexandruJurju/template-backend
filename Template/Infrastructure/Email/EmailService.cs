using Application.Abstractions.Email;
using Domain.Abstractions.Result;
using Domain.Users;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using RazorLight;

namespace Infrastructure.Email;

public class EmailService(IFluentEmail fluentEmail) : IEmailService
{
    public async Task<Result> SendEmail<TModel>(EmailEnvelope envelope, TModel model)
    {
        SendResponse? result = await fluentEmail
            .To(envelope.ToMail)
            .Subject(envelope.Subject)
            .UsingTemplateFromFile(envelope.TemplateName, model, isHtml: true)
            .SendAsync();

        if (!result.Successful)
        {
            return Result.Failure(UserErrors.EmailNotSent());
        }
        
        return Result.Success();
    }
}
