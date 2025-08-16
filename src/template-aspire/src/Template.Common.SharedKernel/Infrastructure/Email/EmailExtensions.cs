using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Common.SharedKernel.Api;

namespace Template.Common.SharedKernel.Infrastructure.Email;

public static class EmailExtensions
{
    public static void AddDefaultFluentEmailWithSmtp(
        this IServiceCollection services,
        IConfiguration configuration,
        string smtpConnectionName)
    {
        services.GetRequiredConfiguration<EmailSettings>(EmailSettings.SectionName);

        EmailSettings settings = services.BuildServiceProvider().GetRequiredService<EmailSettings>();

        services.AddScoped<IEmailService, EmailService>();

        string smtpConnectionString = configuration.GetRequiredConnectionString(smtpConnectionName)!;

        // todo: fix
        var smtpUri = new Uri(smtpConnectionString.Replace("Endpoint=", ""));

        services
            .AddFluentEmail(settings.SenderEmail, settings.Sender)
            .AddSmtpSender(smtpUri.Host, smtpUri.Port)
            .AddRazorRenderer();
    }
}
