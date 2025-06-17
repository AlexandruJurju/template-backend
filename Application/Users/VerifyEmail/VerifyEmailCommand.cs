using Application.Abstractions.Messaging;

namespace Application.Users.VerifyEmail;

public sealed record VerifyEmailCommand(Guid TokenId) : ICommand;
