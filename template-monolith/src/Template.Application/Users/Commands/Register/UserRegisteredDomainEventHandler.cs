using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Template.Application.Abstractions.Email;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.EmailTemplates;
using Template.Domain.Users;
using Template.SharedKernel.Result;

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

        EmailTemplate template =
            await dbContext.EmailTemplates.SingleOrDefaultAsync(x => x.Name == EmailTemplate.UserRegistered, cancellationToken) ??
            throw new Exception(EmailTemplateErrors.NotFound(EmailTemplate.UserRegistered).Description);

        Result result = await emailService.SendEmail(user.Email, template, new RegisterUserMailModel());

        if (result.IsFailure)
        {
            throw new Exception(result.Error.Description);
        }
    }
}
