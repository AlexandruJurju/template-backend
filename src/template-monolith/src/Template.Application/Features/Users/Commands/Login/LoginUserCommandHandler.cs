using Template.Application.Contracts;
using Template.Common.SharedKernel.Application.CQRS.Commands;
using Template.Common.SharedKernel.Infrastructure.Authentication.Jwt;
using Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;
using Template.Domain.Entities.Users;

namespace Template.Application.Features.Users.Commands.Login;

internal sealed class LoginUserCommandHandler(
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    IRepository<User> userRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<LoginUserCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.FirstOrDefaultAsync(x => x.Email == command.Email, [u => u.Role, u => u.Role.Permissions], cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound(command.Email);
        }

        var verified = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return UserErrors.PasswordNotVerified(command.Email);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(tokenProvider.GenerateToken(user));
    }
}
