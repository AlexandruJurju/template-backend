namespace Template.Application.Users.Commands.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginResponse>;
