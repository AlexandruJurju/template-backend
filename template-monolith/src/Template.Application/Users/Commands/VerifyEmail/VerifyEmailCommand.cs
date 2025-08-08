using Template.SharedKernel.Application.Messaging;

namespace Template.Application.Users.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(Guid TokenId) : ICommand;
