namespace Template.Common.SharedKernel.Infrastructure.Auth.Jwt;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
