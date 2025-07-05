using Hangfire;
using Template.Application.Abstractions.Outbox;

namespace Template.API.Extensions;

public static class BackgroundJobExtensions
{
    public static IApplicationBuilder UseBackgroundJobs(this WebApplication app)
    {
        app.Services
            .GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<IProcessOutboxMessagesJob>(
                "outbox-processor",
                job => job.ProcessAsync(),
                app.Configuration["BackgroundJobs:Outbox:Schedule"]);

        return app;
    }
}
