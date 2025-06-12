using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;

namespace Application.Users.Register;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher
)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async ValueTask<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.UserWithEmailExists(request.Email, cancellationToken))
        {
            return UserErrors.EmailNotUnique;
        }

        var user = User.Create(request.Email, request.FirstName, request.LastName, passwordHasher.Hash(request.Password));

        userRepository.Add(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
