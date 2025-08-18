using Template.Application.Contracts;
using Template.Common.Constants.Aspire;
using Template.Common.SharedKernel.Infrastructure.Authentication.Jwt;
using Template.Common.SharedKernel.Infrastructure.Authorization.Jwt;
using Template.Common.SharedKernel.Infrastructure.Caching;
using Template.Common.SharedKernel.Infrastructure.EF;
using Template.Common.SharedKernel.Infrastructure.Email;
using Template.Common.SharedKernel.Infrastructure.MongoDb;
using Template.Common.SharedKernel.Infrastructure.Storage;
using Template.Domain.Abstractions.Persistence;
using Template.Infrastructure.Authentication;
using Template.Infrastructure.Database;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

namespace Template.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDefaultPostgresDb<ApplicationDbContext>(configuration, Components.RelationalDbs.Template);
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddDefaultFluentEmailWithSmtp(configuration, Components.MailPit);

        services.AddDefaultAzureBlobStorage(configuration, Components.Azure.BlobContainer);

        services.AddDefaultCaching(configuration, Components.Redis);

        services.AddDefaultJwtAuthentication();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddDefaultJwtAuthorization();

        services.AddDefaultMongo(configuration, Components.DocumentDbs.Template);

        services.AddTickerQ(options =>
        {
            options.SetMaxConcurrency(Environment.ProcessorCount);
            options.SetInstanceIdentifier(Environment.MachineName);
            options.UpdateMissedJobCheckDelay(TimeSpan.FromMinutes(5));
            options.AddOperationalStore<ApplicationDbContext>(efOpt =>
            {
                efOpt.UseModelCustomizerForMigrations();
                efOpt.CancelMissedTickersOnApplicationRestart();
            });
            options.AddDashboard("/tickerq").AddDashboardBasicAuth();
        });
    }
}
