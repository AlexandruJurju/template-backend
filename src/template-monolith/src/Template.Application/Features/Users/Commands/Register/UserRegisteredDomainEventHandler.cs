using Microsoft.Extensions.Logging;
using Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Commands.Register;

internal sealed class UserRegisteredDomainEventHandler(
    ILogger<UserRegisteredDomainEventHandler> logger,
    IRepository<User> userRepository
    // IEmailService emailService,
    // IUnitOfWork unitOfWork
) : INotificationHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        User? user = await userRepository.FirstOrDefaultAsync(x => x.Id == notification.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        logger.LogInformation("User {UserEmail} registered", user.Email);

        // await emailService.SendEmail(user.Email, "User Registered", EmailTemplates.UserRegistered, new RegisterUserMailModel());
    }
}
