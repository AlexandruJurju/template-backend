using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Common.SharedKernel.Api;
using Template.Common.SharedKernel.Infrastructure.Configuration;

namespace Template.Common.SharedKernel.Infrastructure.Email;

public static class EmailExtensions
{
    public static void AddDefaultFluentEmailWithSmtp(
        this IServiceCollection services,
        IConfiguration configuration,
        string smtpConnectionName)
    {
        services.AddOptionsWithValidation<EmailOptions>(EmailOptions.SectionName);

        EmailOptions options = services.BuildServiceProvider().GetRequiredService<EmailOptions>();

        services.AddScoped<IEmailService, EmailService>();

        var smtpConnectionString = configuration.GetConnectionStringOrThrow(smtpConnectionName)!;

        // todo: fix
        var smtpUri = new Uri(smtpConnectionString.Replace("Endpoint=", ""));

        services
            .AddFluentEmail(options.SenderEmail, options.Sender)
            .AddSmtpSender(smtpUri.Host, smtpUri.Port)
            .AddRazorRenderer();
    }
}
