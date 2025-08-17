namespace Template.Common.SharedKernel.Infrastructure.Authentication.Jwt;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
