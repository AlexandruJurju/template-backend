using Template.Application.Abstractions.Email;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Users;

namespace Template.Application.Users.Commands.Register;

internal sealed class UserRegisteredDomainEventHandler(
    IApplicationDbContext dbContext,
    IEmailService emailService
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

        Result result = await emailService.SendEmail(user.Email, "User Registered", EmailTemplates.UserRegistered, new RegisterUserMailModel());

        if (!result.IsSuccess)
        {
            throw new Exception();
        }
    }
}
