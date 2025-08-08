using Template.Application.Abstractions.Messaging;

namespace Template.Application.Users.Commands.Register;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName, string Password)
    : ICommand<Guid>;
