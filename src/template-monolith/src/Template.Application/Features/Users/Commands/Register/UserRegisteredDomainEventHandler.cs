using Microsoft.Extensions.Logging;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Commands.Register;

internal sealed class UserRegisteredDomainEventHandler(
    IApplicationDbContext dbContext,
    ILogger<UserRegisteredDomainEventHandler> logger
    // IEmailService emailService
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

        logger.LogInformation("User {UserEmail} registered", user.Email);

        // await emailService.SendEmail(user.Email, "User Registered", EmailTemplates.UserRegistered, new RegisterUserMailModel());
    }
}
