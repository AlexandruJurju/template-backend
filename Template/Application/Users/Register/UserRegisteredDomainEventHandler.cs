using Application.Abstractions.Email;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.EmailTemplates;
using Domain.Users;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler(
    IEmailService emailService,
    IUserRepository userRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IEmailTemplateRepository emailTemplateRepository,
    IEmailVerificationLinkFactory emailVerificationLinkFactory,
    TimeProvider timeProvider,
    IConfiguration configuration
) : INotificationHandler<UserRegisteredDomainEvent>
{
    public async ValueTask Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(notification.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        var token = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CreatedOnUtc = timeProvider.GetUtcNow().UtcDateTime,
            ExpiresOnUtc = timeProvider.GetUtcNow().UtcDateTime.AddDays(configuration.GetValue<int>("Email:VerificationTokenExpireHours"))
        };
        emailVerificationTokenRepository.Add(token);

        string verificationLink = emailVerificationLinkFactory.Create(token);

        EmailTemplate template = await emailTemplateRepository.GetByNameAsync(EmailTemplate.UserRegistered, cancellationToken) ??
                                 throw new Exception(EmailTemplateErrors.NotFound(EmailTemplate.UserRegistered).Description);

        Result result = await emailService.SendEmail(user.Email, template, new RegisterUserMailModel(verificationLink));

        if (result.IsFailure)
        {
            throw new Exception(result.Error.Description);
        }
    }
}
