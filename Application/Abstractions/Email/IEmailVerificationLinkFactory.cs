using Domain.Users;

namespace Application.Abstractions.Email;

public interface IEmailVerificationLinkFactory
{
    string Create(EmailVerificationToken emailVerificationToken);
}
