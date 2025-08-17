using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

namespace Template.Infrastructure;

// DOESN'T WORK FOR GENERIC TDbContext
public static class TickerQExtensions
{
    public static void AddDefaultTickerQ<TDbContext>(
        this IServiceCollection services,
        int maxConcurrency = 4,
        bool addDashboard = true,
        bool addDashboardAuth = false
    )
        where TDbContext : DbContext
    {
        services.AddTickerQ(options =>
        {
            options.SetMaxConcurrency(maxConcurrency);
            options.AddOperationalStore<TDbContext>(efOpt =>
            {
                efOpt.UseModelCustomizerForMigrations();
                efOpt.CancelMissedTickersOnApplicationRestart();
            });

            if (!addDashboard)
            {
                return;
            }

            options.AddDashboard("/tickerq-dashboard");

            if (addDashboardAuth)
            {
                options.AddDashboardBasicAuth();
            }
        });
    }
}
