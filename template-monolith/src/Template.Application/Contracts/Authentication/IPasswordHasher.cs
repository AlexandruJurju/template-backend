namespace Template.Application.Contracts.Authentication;

// this could also be a domain service
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
