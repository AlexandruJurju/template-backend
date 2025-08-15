using Keycloak.Net;
using Keycloak.Net.Models.Users;
using Template.Application.Contracts.Authentication;

namespace Template.Infrastructure.Authentication;

public class KeycloakService : IKeycloakService
{
    private readonly ILogger<KeycloakService> _logger;
    private readonly string _realm;
    private readonly string _keycloakUrl;
    private readonly string _clientSecret;
    private readonly KeycloakOptions _options;

    public KeycloakService(IConfiguration configuration, ILogger<KeycloakService> logger)
    {
        _logger = logger;

        _keycloakUrl = configuration["Keycloak:Url"]!;
        string clientId = configuration["Keycloak:ClientId"]!;
        _clientSecret = configuration["Keycloak:ClientSecret"]!;
        _realm = configuration["Keycloak:Realm"]!;

        if (string.IsNullOrEmpty(_keycloakUrl) || string.IsNullOrEmpty(clientId) ||
            string.IsNullOrEmpty(_clientSecret) || string.IsNullOrEmpty(_realm))
        {
            throw new ArgumentException("Keycloak configuration is incomplete. Required: Url, ClientId, ClientSecret, Realm");
        }

        // Configure KeycloakOptions with your client ID
        _options = new KeycloakOptions(
            prefix: "",
            adminClientId: clientId, // Use your service account client ID
            authenticationRealm: _realm
        );
    }

    private KeycloakClient CreateKeycloakClient()
    {
        // Use the client secret constructor with proper options
        var client = new KeycloakClient(_keycloakUrl, _clientSecret, _options);
        return client;
    }

    public async Task<Domain.Entities.Users.User> CreateUserAsync(
        string username,
        string email,
        string firstName,
        string lastName,
        string password)
    {
        try
        {
            KeycloakClient keycloakClient = CreateKeycloakClient();

            var user = new User
            {
                UserName = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Enabled = true,
                EmailVerified = false,
                Attributes = new Dictionary<string, IEnumerable<string>>()
            };

            bool result = await keycloakClient.CreateUserAsync(_realm, user);

            if (!result)
            {
                _logger.LogError("Failed to create user {Username}", username);
                return null;
            }

            User createdUser = await GetUserByUsernameAsync(username);

            // if (createdUser != null && !string.IsNullOrEmpty(password))
            // {
            //     await SetUserPasswordAsync(createdUser.Id, password, false);
            // }

            return Domain.Entities.Users.User.Create(
                createdUser.Id,
                createdUser.Email,
                createdUser.FirstName,
                createdUser.LastName);
        }
#pragma warning disable S2139
        catch (Exception ex)
#pragma warning restore S2139
        {
            _logger.LogError(ex, "Error creating user {Username}", username);
            throw;
        }
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        try
        {
            KeycloakClient keycloakClient = CreateKeycloakClient();
            IEnumerable<User>? users = await keycloakClient.GetUsersAsync(_realm, username: username);
            return users?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by username {Username}", username);
            return null;
        }
    }

    // public async Task<bool> SetUserPasswordAsync(string userId, string password, bool temporary = false)
    // {
    //     try
    //     {
    //         KeycloakClient keycloakClient = CreateKeycloakClient();
    //         return await keycloakClient.SetUserPasswordAsync(_realm, userId, password, temporary);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error setting password for user {UserId}", userId);
    //         return false;
    //     }
    // }
}
