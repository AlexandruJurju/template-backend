using Microsoft.EntityFrameworkCore;
using Template.Application.Abstractions.Authentication;
using Template.Application.Abstractions.Messaging;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Abstractions.Result;
using Template.Domain.Users;

namespace Template.Application.Users.Commands.Register;

public sealed class RegisterUserCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher
)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(user => user.Email == request.Email, cancellationToken))
        {
            return UserErrors.EmailNotUnique;
        }

        var user = User.Create(request.Email, request.FirstName, request.LastName, passwordHasher.Hash(request.Password));

        dbContext.Attach(user.Role);
        
        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
