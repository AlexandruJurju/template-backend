using Template.Domain.EmailTemplates;
using Template.SharedKernel.Result;

namespace Template.Application.Abstractions.Email;

public interface IEmailService
{
    Task<Result> SendEmail<TModel>(string toMail, EmailTemplate emailTemplate, TModel model);
}
