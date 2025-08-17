using Microsoft.Extensions.Logging;
using Template.Application.Features.Users.Commands.Register;
using Template.Common.SharedKernel.Application.EventBus;

namespace Template.Application.Features.Users;

public sealed class UserRegisteredIntegrationEventHandler(
    ILogger<UserRegisteredIntegrationEventHandler> logger
) : IntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    public override async Task Handle(UserRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("UserRegisteredIntegrationEvent");

        await Task.CompletedTask;
    }
}
