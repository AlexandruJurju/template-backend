using Template.Common.SharedKernel.Application.EventBus;

namespace Template.Application.Features.Users.Commands.Register;

public sealed record UserRegisteredIntegrationEvent(string Email) : IIntegrationEvent;
