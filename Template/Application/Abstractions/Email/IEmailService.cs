using Domain.Abstractions.Result;

namespace Application.Abstractions.Email;

public interface IEmailService
{
    Task<Result> SendEmail<TModel>(EmailEnvelope envelope, TModel model);
}
