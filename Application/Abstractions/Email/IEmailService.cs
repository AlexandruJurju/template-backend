using Domain.Abstractions.Result;
using Domain.EmailTemplates;

namespace Application.Abstractions.Email;

public interface IEmailService
{
    Task<Result> SendEmail<TModel>(string toMail, EmailTemplate emailTemplate, TModel model);
}
