using System.Text;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Template.Application.Abstractions.Authentication;
using Template.Application.Abstractions.Email;
using Template.Application.Abstractions.Outbox;
using Template.Application.Abstractions.Storage;
using Template.Domain.Abstractions.Persistence;
using Template.Infrastructure.Authentication;
using Template.Infrastructure.Authorization;
using Template.Infrastructure.Data;
using Template.Infrastructure.Email;
using Template.Infrastructure.Outbox;
using Template.Infrastructure.Storage;
using Template.SharedKernel.Infrastructure.Data;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

namespace Template.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDatabase(services, configuration);

        AddCaching(services, configuration);

        AddTickerQ(services);

        AddAuthenticationInternal(services, configuration);

        AddAuthorizationInternal(services);

        AddEmail(services, configuration);

        AddStorage(services, configuration);

        return services;
    }

    private static void AddEmail(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailOptions>(configuration.GetSection("Authentication"));

        services.AddScoped<IEmailService, EmailService>();

        // Get the Papercut connection string
        string papercutConnectionString = configuration.GetConnectionString("papercut")!;
        var smtpUri = new Uri(papercutConnectionString.Replace("Endpoint=", ""));

        services
            .AddFluentEmail(configuration["Email:SenderEmail"], configuration["Email:Sender"])
            .AddSmtpSender(smtpUri.Host, smtpUri.Port)
            .AddRazorRenderer();
    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("cache")!;

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
            options.AddDashboard(basePath: "/tickerq-dashboard");
            options.AddDashboardBasicAuth();
        });

        services.AddScoped<IProcessOutboxMessagesJob, ProcessOutboxMessagesJob>();
    }

    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("database");

        services.AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString!));
    }

    private static void AddAuthenticationInternal(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt")!);

        JwtOptions jwtOptions = configuration
            .GetSection("Jwt")
            .Get<JwtOptions>()!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

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
        services.AddSingleton(_ => new BlobServiceClient(configuration.GetConnectionString("blob-storage")!));
    }
}
