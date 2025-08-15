namespace Template.Application.Contracts.Email;

public interface IEmailService
{
    Task<Result> SendEmail<TModel>(string toMail, string subject, string emailTemplateName, TModel model);
}
