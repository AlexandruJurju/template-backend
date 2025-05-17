using Application.Abstractions.Email;
using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler(
    IEmailService emailService,
    IApplicationDbContext dbContext,
    IEmailVerificationLinkFactory emailVerificationLinkFactory
) : INotificationHandler<UserRegisteredDomainEvent>
{
    public async ValueTask Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == notification.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        // todo: configure expire from config
        var token = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CreatedOnUtc = TimeProvider.System.GetUtcNow().UtcDateTime,
            ExpiresOnUtc = TimeProvider.System.GetUtcNow().UtcDateTime.AddDays(1)
        };
        dbContext.EmailVerificationTokens.Add(token);

        string verificationLink = emailVerificationLinkFactory.Create(token);

        var envelope = new EmailEnvelope(user.Email, "Registered", "EmailTemplates/UserRegistered.cshtml");

        // todo: save templates in db
        Result result = await emailService.SendEmail(envelope, new RegisterUserMailModel(verificationLink));

        if (result.IsFailure)
        {
            throw new Exception(result.Error.Description);
        }
    }
}
