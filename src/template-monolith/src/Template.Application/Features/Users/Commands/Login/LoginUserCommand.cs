using Template.Common.SharedKernel.Application.CQRS.Commands;

namespace Template.Application.Features.Users.Commands.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginResponse>;
