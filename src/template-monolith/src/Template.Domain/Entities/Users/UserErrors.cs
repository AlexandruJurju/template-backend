namespace Template.Domain.Entities.Users;

public static class UserErrors
{
    public static Result RefreshTokenExpired =>
        Result.Error("Users.RefreshTokenExpired: The refresh token has expired");

    public static Result EmailNotUnique(string email)
    {
        return Result.Conflict($"Users.EmailNotUnique: The provided email: {email} is not unique");
    }

    public static Result EmailVerificationTokenNotFound()
    {
        return Result.NotFound("Users.EmailVerificationTokenNotFound: No email verification token was found for the provided email");
    }

    public static Result EmailNotSent()
    {
        return Result.Error("Users.EmailNotSent: Email could not be sent");
    }

    public static Result NotFound(Guid userId)
    {
        return Result.NotFound($"Users.NotFound: The user with the Id: '{userId}' was not found");
    }

    public static Result NotFound(string email)
    {
        return Result.NotFound($"Users.NotFound: The user with the Email: '{email}' was not found");
    }

    public static Result PasswordNotVerified(string email)
    {
        return Result.NotFound($"Users.PasswordNotVerified: The user with the Email: '{email}' doesn't have a verified password");
    }

    public static Result Unauthorized()
    {
        return Result.Unauthorized("Users.Unauthorized: You are not authorized to perform this action.");
    }
}
