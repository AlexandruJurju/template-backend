using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using Template.Application.Contracts;
using Template.Common.Constants.Aspire;
using Template.Common.SharedKernel.Infrastructure.Auth.Jwt;
using Template.Common.SharedKernel.Infrastructure.Caching;
using Template.Common.SharedKernel.Infrastructure.Dapper;
using Template.Common.SharedKernel.Infrastructure.EF;
using Template.Common.SharedKernel.Infrastructure.Email;
using Template.Common.SharedKernel.Infrastructure.Outbox;
using Template.Common.SharedKernel.Infrastructure.Repository;
using Template.Common.SharedKernel.Infrastructure.Storage;
using Template.Domain.Abstractions.Persistence;
using Template.Infrastructure.Authentication;
using Template.Infrastructure.Authorization;
using Template.Infrastructure.Outbox;
using Template.Infrastructure.Persistence;
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

        services.AddDefaultFluentEmailWithSmtp(configuration, Components.MailPit);

        services.AddDefaultAzureBlobStorage(configuration, Components.Azure.BlobContainer);

        services.AddDefaultCaching(configuration, Components.Valkey);

        builder.AddDefaultPostgresDb<ApplicationDbContext>(Components.Database.Template);
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        AddTickerQ(services);

        services.AddDefaultDapper(configuration, Components.Database.Template);

        services.AddDefaultJwtAuthentication();
        services.AddScoped<ITokenProvider, TokenProvider>();

        AddAuthorizationInternal(services);
    }

    private static void AddAuthorizationInternal(IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddScoped<PermissionProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
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
        });

        services.AddScoped<IProcessOutboxMessagesJob, ProcessOutboxMessagesJob>();
    }
}
