using Template.Common.SharedKernel.Application.CQRS.Commands;
using Template.Common.SharedKernel.Infrastructure.Authentication.Jwt;
using Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Commands.Register;

internal sealed class RegisterUserCommandHandler(
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IRepository<User> userRepository
) : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // todo: cache
        if (await userRepository.AnyAsync(user => user.Email == request.Email, cancellationToken))
        {
            return UserErrors.EmailNotUnique(request.Email);
        }

        var user = User.Create(request.Email, request.FirstName, request.LastName, passwordHasher.Hash(request.Password));

        userRepository.Add(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
