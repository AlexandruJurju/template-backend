using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Domain.Abstractions.Result;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Login;

public sealed class LoginUserCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public LoginUserCommandHandler(ITokenProvider tokenProvider, IPasswordHasher passwordHasher, IApplicationDbContext context)
    {
        _tokenProvider = tokenProvider;
        _passwordHasher = passwordHasher;
        _context = context;
    }

    public async Task<Result<string>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<string>(UserErrors.NotFoundByEmail);
        }

        var verified = _passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<string>(UserErrors.NotFoundByEmail);
        }

        var token = _tokenProvider.Create(user);

        return token;
    }
}