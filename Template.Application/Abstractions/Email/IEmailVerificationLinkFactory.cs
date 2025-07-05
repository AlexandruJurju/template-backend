using Template.Domain.Users;

namespace Template.Application.Abstractions.Email;

public interface IEmailVerificationLinkFactory
{
    string Create(EmailVerificationToken emailVerificationToken);
}
