using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Template.Application.Abstractions.Email;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Abstractions.Result;
using Template.Domain.EmailTemplates;
using Template.Domain.Users;

namespace Template.Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler(
    IApplicationDbContext dbContext,
    IEmailService emailService,
    IEmailVerificationLinkFactory emailVerificationLinkFactory,
    TimeProvider timeProvider,
    IConfiguration configuration
) : INotificationHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == notification.UserId, cancellationToken);

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
        dbContext.EmailVerificationTokens.Add(token);

        string verificationLink = emailVerificationLinkFactory.Create(token);

        EmailTemplate template =
            await dbContext.EmailTemplates.SingleOrDefaultAsync(x => x.Name == EmailTemplate.UserRegistered, cancellationToken) ??
            throw new Exception(EmailTemplateErrors.NotFound(EmailTemplate.UserRegistered).Description);

        Result result = await emailService.SendEmail(user.Email, template, new RegisterUserMailModel(verificationLink));

        if (result.IsFailure)
        {
            throw new Exception(result.Error.Description);
        }
    }
}
