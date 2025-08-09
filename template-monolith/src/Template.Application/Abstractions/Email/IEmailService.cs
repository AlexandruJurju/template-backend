using Template.Domain.EmailTemplates;

namespace Template.Application.Abstractions.Email;

public interface IEmailService
{
    Task<Result> SendEmail<TModel>(string toMail, EmailTemplate emailTemplate, TModel model);
}
