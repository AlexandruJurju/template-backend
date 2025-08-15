namespace Template.Application.Features.Users.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(Guid TokenId) : ICommand;
