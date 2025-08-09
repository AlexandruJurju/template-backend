using Ardalis.Result;

namespace Template.Domain.Users;

public static class UserErrors
{
    // 409
    public static Result EmailNotUnique(string email) =>
        Result.Conflict($"Users.EmailNotUnique: The provided email: {email} is not unique");

    // 404
    public static Result EmailVerificationTokenNotFound() =>
        Result.NotFound("Users.EmailVerificationTokenNotFound: No email verification token was found for the provided email");

    // 500 (generic error)
    public static Result RefreshTokenExpired =>
        Result.Error("Users.RefreshTokenExpired: The refresh token has expired");

    // 500 (generic error)
    public static Result EmailNotSent() =>
        Result.Error("Users.EmailNotSent: Email could not be sent");

    // 404
    public static Result NotFound(Guid userId) =>
        Result.NotFound($"Users.NotFound: The user with the Id: '{userId}' was not found");

    // 404
    public static Result NotFound(string email) =>
        Result.NotFound($"Users.NotFound: The user with the Email: '{email}' was not found");

    // 401
    public static Result Unauthorized() =>
        Result.Unauthorized("Users.Unauthorized: You are not authorized to perform this action.");
}
