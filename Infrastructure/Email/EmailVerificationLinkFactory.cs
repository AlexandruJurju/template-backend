using Application.Abstractions.Email;
using Domain.Users;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Email;

public class EmailVerificationLinkFactory(IConfiguration configuration) : IEmailVerificationLinkFactory
{
    public string Create(EmailVerificationToken emailVerificationToken)
    {
        string baseUrl = configuration["BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured");

        baseUrl = baseUrl.TrimEnd('/');

        return $"{baseUrl}/api/users/verify-email?token={emailVerificationToken.Id}";
    }
}
