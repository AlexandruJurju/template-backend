using Application.Abstractions.Email;
using Application.Abstractions.Persistence;
using Domain.Users;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler(
    IEmailService emailService,
    IApplicationDbContext dbContext
) : INotificationHandler<UserRegisteredDomainEvent>
{
    public async ValueTask Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == notification.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        var envelope = new EmailEnvelope(user.Email, "Registered", "You were registered!");

        await emailService.SendEmail(envelope, "EmailTemplates/UserRegistered.cshtml", user);
    }
}
