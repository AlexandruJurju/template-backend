using Application.Abstractions.Email;
using FluentEmail.Core;
using RazorLight;

namespace Infrastructure.Email;

public class EmailService(IFluentEmail fluentEmail) : IEmailService
{
    public async Task SendEmail<TModel>(EmailEnvelope envelope, string templatePath, TModel model)
    {
        await fluentEmail
            .To(envelope.ToMail)
            .Subject(envelope.Subject)
            .UsingTemplateFromFile(templatePath, model, isHtml: true)
            .SendAsync();
    }
}
