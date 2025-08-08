using Template.Domain.EmailTemplates;
using Template.SharedKernel.Application.CustomResult;

namespace Template.Application.Abstractions.Email;

public interface IEmailService
{
    Task<Result> SendEmail<TModel>(string toMail, EmailTemplate emailTemplate, TModel model);
}
