using Quartz;

namespace Template.Infrastructure.BackgroundJobs;

public class ProcessOutboxMessagesJobSetup(BackgroundJobsOptions backgroundJobsOptions)
    : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        string jobName = nameof(ProcessOutboxMessagesJob);

        options.AddJob<ProcessOutboxMessagesJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule
                            .WithIntervalInMinutes(backgroundJobsOptions.Outbox.ScheduleInMinutes)
                            .RepeatForever()
                    )
            );
    }
}
