using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Register;

public sealed class RegisterUserCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }

        var user = User.Create(command.Email, command.FirstName, command.LastName, passwordHasher.Hash(command.Password));

        context.Attach(user.Role!);

        context.Users.Add(user);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
