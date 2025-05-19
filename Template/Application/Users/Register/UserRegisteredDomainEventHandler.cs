using Application.Abstractions.Email;
using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.EmailTemplates;
using Domain.Users;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler(
    IEmailService emailService,
    IApplicationDbContext dbContext,
    IEmailVerificationLinkFactory emailVerificationLinkFactory,
    IConfiguration configuration
) : INotificationHandler<UserRegisteredDomainEvent>
{
    public async ValueTask Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == notification.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        var token = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CreatedOnUtc = TimeProvider.System.GetUtcNow().UtcDateTime,
            ExpiresOnUtc = TimeProvider.System.GetUtcNow().UtcDateTime.AddDays(configuration.GetValue<int>("Email:VerificationTokenExpireHours"))
        };
        dbContext.EmailVerificationTokens.Add(token);

        string verificationLink = emailVerificationLinkFactory.Create(token);

        EmailTemplate? template = await dbContext.EmailTemplates.SingleOrDefaultAsync(e => e.Name == EmailTemplate.UserRegistered, cancellationToken) ??
                                  throw new Exception(EmailTemplateErrors.NotFound(EmailTemplate.UserRegistered).Description);

        Result result = await emailService.SendEmail(user.Email, template, new RegisterUserMailModel(verificationLink));

        if (result.IsFailure)
        {
            throw new Exception(result.Error.Description);
        }
    }
}
