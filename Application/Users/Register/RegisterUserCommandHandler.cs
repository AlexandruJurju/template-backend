using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Register;

public sealed class RegisterUserCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher
)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async ValueTask<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(user => user.Email == request.Email, cancellationToken))
        {
            return UserErrors.EmailNotUnique;
        }

        var user = User.Create(request.Email, request.FirstName, request.LastName, passwordHasher.Hash(request.Password));

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}