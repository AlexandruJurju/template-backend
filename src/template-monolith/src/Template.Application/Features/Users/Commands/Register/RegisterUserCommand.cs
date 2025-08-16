using Template.Common.SharedKernel.Application.CQRS.Commands;

namespace Template.Application.Features.Users.Commands.Register;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName, string Password)
    : ICommand<Guid>;
