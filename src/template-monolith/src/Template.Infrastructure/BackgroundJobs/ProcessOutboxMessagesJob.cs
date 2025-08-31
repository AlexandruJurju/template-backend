using MediatR;
using Newtonsoft.Json;
using Quartz;
using Template.Common.SharedKernel.Infrastructure.Helpers;
using Template.Common.SharedKernel.Infrastructure.Outbox;
using Template.Common.SharedKernel.Infrastructure.Serialization;
using Template.Domain.Abstractions.Persistence;

namespace Template.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob(
    IMediator mediator,
    IApplicationDbContext applicationDbContext,
    ILogger<ProcessOutboxMessagesJob> logger
) : IJob
{
    private const int BatchSize = 1000;

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Starting to process outbox messages");

        List<OutboxMessage> outboxMessages = await applicationDbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .Take(BatchSize)
            .OrderBy(m => m.OccurredOnUtc)
            .ToListAsync();

        if (outboxMessages.Count == 0)
        {
            logger.LogInformation("No outbox messages to process");
            return;
        }

        foreach (OutboxMessage outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, SerializerSettings.Instance)!;
                await mediator.Publish(domainEvent);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Exception while processing outbox message {MessageId}",
                    outboxMessage.Id
                );
                exception = ex;
            }

            // Update message status (whether success or failed)
            outboxMessage.ProcessedOnUtc = DateTimeHelper.UtcNow();
            outboxMessage.Error = exception?.ToString();
        }

        await applicationDbContext.SaveChangesAsync();
    }
}
