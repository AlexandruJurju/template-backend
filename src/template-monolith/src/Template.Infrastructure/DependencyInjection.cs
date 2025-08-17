using Microsoft.Extensions.Hosting;
using Template.Application.Contracts;
using Template.Common.Constants.Aspire;
using Template.Common.SharedKernel.Infrastructure.Authentication.Jwt;
using Template.Common.SharedKernel.Infrastructure.Authorization.Jwt;
using Template.Common.SharedKernel.Infrastructure.Caching;
using Template.Common.SharedKernel.Infrastructure.Dapper;
using Template.Common.SharedKernel.Infrastructure.EF;
using Template.Common.SharedKernel.Infrastructure.Email;
using Template.Common.SharedKernel.Infrastructure.Outbox;
using Template.Common.SharedKernel.Infrastructure.Storage;
using Template.Domain.Abstractions.Persistence;
using Template.Infrastructure.Authentication;
using Template.Infrastructure.Database;
using Template.Infrastructure.Jobs;
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

        builder.AddDefaultPostgresDb<ApplicationDbContext>(Components.Database.Template);
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddDefaultFluentEmailWithSmtp(configuration, Components.MailPit);

        services.AddDefaultAzureBlobStorage(configuration, Components.Azure.BlobContainer);

        services.AddDefaultCaching(configuration, Components.Redis);

        services.AddDefaultDapper(configuration, Components.Database.Template);

        services.AddDefaultJwtAuthentication();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddDefaultJwtAuthorization();

        services.AddTickerQ(opt =>
        {
            opt.SetMaxConcurrency(Environment.ProcessorCount);
            opt.SetInstanceIdentifier(Environment.MachineName);
            opt.UpdateMissedJobCheckDelay(TimeSpan.FromMinutes(5));
            opt.AddOperationalStore<ApplicationDbContext>(efOpt =>
            {
                efOpt.UseModelCustomizerForMigrations();
                efOpt.CancelMissedTickersOnApplicationRestart();
            });
            opt.AddDashboard("/tickerq").AddDashboardBasicAuth();
        });
    }
}
