using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(Guid TokenId) : ICommand;
