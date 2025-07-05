using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;
