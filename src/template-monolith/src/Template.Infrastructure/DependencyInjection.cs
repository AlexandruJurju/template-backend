using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Hosting;
using Template.Application.Contracts.Authentication;
using Template.Application.Contracts.Email;
using Template.Application.Contracts.Storage;
using Template.Common.Constants.Aspire;
using Template.Common.SharedKernel.Infrastructure;
using Template.Common.SharedKernel.Infrastructure.Dapper;
using Template.Common.SharedKernel.Infrastructure.EF;
using Template.Common.SharedKernel.Infrastructure.Outbox;
using Template.Common.SharedKernel.Infrastructure.Repository;
using Template.Domain.Abstractions.Persistence;
using Template.Infrastructure.Authentication;
using Template.Infrastructure.Authorization;
using Template.Infrastructure.Email;
using Template.Infrastructure.Outbox;
using Template.Infrastructure.Persistence;
using Template.Infrastructure.Persistence.Dapper;
using Template.Infrastructure.Persistence.GenericRepository;
using Template.Infrastructure.Storage;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

namespace Template.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        IConfigurationManager configuration = builder.Configuration;

        AddDatabase(builder);

        AddCaching(services, configuration);

        AddTickerQ(services);

        AddAuthenticationInternal(services);

        AddAuthorizationInternal(services);

        AddEmail(services, configuration);

        AddStorage(services, configuration);
    }

    private static void AddEmail(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailOptions>(configuration.GetSection("Email"));

        services.AddScoped<IEmailService, EmailService>();

        // Get smtp connection string
        string smtpConnectionString = configuration.GetConnectionString(Components.MailPit)!;
        var smtpUri = new Uri(smtpConnectionString.Replace("Endpoint=", ""));

        services
            .AddFluentEmail(configuration["Email:SenderEmail"], configuration["Email:Sender"])
            .AddSmtpSender(smtpUri.Host, smtpUri.Port)
            .AddRazorRenderer();
    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString(Components.Valkey)!;

        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);

        services.AddHybridCache(options =>
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                // Local cache expiration
                LocalCacheExpiration = TimeSpan.FromMinutes(1),
                // Distributed cache expiration
                Expiration = TimeSpan.FromMinutes(5)
            });
    }

    private static void AddTickerQ(IServiceCollection services)
    {
        services.AddTickerQ(options =>
        {
            options.SetMaxConcurrency(4);
            options.AddOperationalStore<ApplicationDbContext>(efOpt =>
            {
                efOpt.UseModelCustomizerForMigrations();
                efOpt.CancelMissedTickersOnApplicationRestart();
            });
            options.AddDashboard();
            options.AddDashboardBasicAuth();
        });

        services.AddScoped<IProcessOutboxMessagesJob, ProcessOutboxMessagesJob>();
    }

    private static void AddDatabase(IHostApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;

        builder.AddPostgresDbContext<ApplicationDbContext>(Components.Database.Template);

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(builder.Configuration.GetConnectionString(Components.Database.Template)!)
        );

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    }

    private static void AddAuthenticationInternal(IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakJwtBearer(
                Components.KeyCloak,
                "template-realm",
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = "account";
                }
            );

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
    }

    private static void AddAuthorizationInternal(IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddScoped<PermissionProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }

    private static void AddStorage(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBlobStorage, BlobStorage>();
        services.AddSingleton(_ => new BlobServiceClient(configuration.GetConnectionString(Components.Azure.BlobContainer)!));
    }
}
