using Template.Domain.Entities.Users;

namespace Template.Application.Contracts.Authentication;

public interface IKeycloakService
{
    Task<User> CreateUserAsync(string username, string email, string firstName, string lastName, string password);
}
