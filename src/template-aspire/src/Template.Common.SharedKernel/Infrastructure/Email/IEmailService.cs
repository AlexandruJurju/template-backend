namespace Template.Common.SharedKernel.Infrastructure.Email;

public interface IEmailService
{
    Task SendEmail<TModel>(string toMail, string subject, string emailTemplateName, TModel model);
}
