using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginResponse>;
