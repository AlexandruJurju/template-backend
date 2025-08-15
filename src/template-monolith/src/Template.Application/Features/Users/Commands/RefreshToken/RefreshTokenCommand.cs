namespace Template.Application.Features.Users.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;
