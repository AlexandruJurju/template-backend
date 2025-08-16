using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Template.Common.SharedKernel.Infrastructure.Email;

public class EmailService(IFluentEmail fluentEmail) : IEmailService
{
    public async Task SendEmail<TModel>(string toMail, string subject, string emailTemplateName, TModel model)
    {
        SendResponse? result = await fluentEmail
            .To(toMail)
            .Subject(subject)
            .UsingTemplateFromFile(emailTemplateName, model, true)
            .SendAsync();

        if (!result.Successful)
        {
            throw new NotImplementedException();
        }
    }
}
