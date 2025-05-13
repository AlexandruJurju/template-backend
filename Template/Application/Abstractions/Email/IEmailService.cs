namespace Application.Abstractions.Email;

public interface IEmailService
{
    Task SendEmail<TModel>(EmailEnvelope envelope, string templatePath, TModel model);
}
