using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;
