using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.VerifyEmail;

public sealed record VerifyEmailCommand(Guid TokenId) : ICommand;
