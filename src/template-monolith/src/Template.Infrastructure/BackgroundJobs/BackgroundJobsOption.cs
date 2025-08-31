using System.ComponentModel.DataAnnotations;

namespace Template.Infrastructure.BackgroundJobs;

[OptionsValidator]
public sealed partial class BackgroundJobsOptions : IValidateOptions<BackgroundJobsOptions>
{
    internal const string SectionName = "BackgroundJobs";

    [Required] [ValidateObjectMembers] public OutboxOptions Outbox { get; init; } = new();
}

public sealed class OutboxOptions
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ScheduleInMinutes must be greater than 0.")]
    public int ScheduleInMinutes { get; init; }
}
