using Microsoft.Extensions.Hosting;
using Quartz;
using Template.Application.Contracts;
using Template.Common.Constants.Aspire;
using Template.Common.SharedKernel.Infrastructure.Authentication.Jwt;
using Template.Common.SharedKernel.Infrastructure.Authorization.Jwt;
using Template.Common.SharedKernel.Infrastructure.Caching;
using Template.Common.SharedKernel.Infrastructure.Configuration;
using Template.Common.SharedKernel.Infrastructure.Email;
using Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework;
using Template.Common.SharedKernel.Infrastructure.Persistence.Mongo;
using Template.Common.SharedKernel.Infrastructure.Storage;
using Template.Domain.Abstractions.Persistence;
using Template.Infrastructure.Authentication;
using Template.Infrastructure.BackgroundJobs;
using Template.Infrastructure.Database;

namespace Template.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IHostApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;

        builder.AddDefaultPostgresDb<ApplicationDbContext>(
            Components.RelationalDbs.Template,
            hostBuilder =>
            {
                if (hostBuilder.Environment.IsDevelopment())
                {
                    services.AddMigration<ApplicationDbContext, ApplicationDbContextSeeder>();
                }
                else
                {
                    services.AddMigration<ApplicationDbContext>();
                }
            }
        );

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddDefaultFluentEmailWithSmtp(configuration, Components.MailPit);

        services.AddDefaultAzureBlobStorage(configuration, Components.Azure.BlobContainer);

        services.AddDefaultCaching(configuration, Components.Redis);

        services.AddDefaultJwtAuthentication();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddDefaultJwtAuthorization();

        services.AddDefaultMongo(configuration, Components.DocumentDbs.Template);

        // Custom
        services.AddBackgroundJobs();
    }

    private static void AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddOptionsWithValidation<BackgroundJobsOptions>(BackgroundJobsOptions.SectionName);

        services.AddQuartz();

        services.AddQuartzHostedService();

        services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();
    }
}
