using Microsoft.EntityFrameworkCore;
using Template.Application.Abstractions.Authentication;
using Template.Domain.Abstractions.Persistence;
using Template.Domain.Users;
using Template.SharedKernel.Application.CustomResult;
using Template.SharedKernel.Application.Messaging;

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
