using MediatR;
using Newtonsoft.Json;
using Template.Common.SharedKernel.Infrastructure.Helpers;
using Template.Common.SharedKernel.Infrastructure.Outbox;
using Template.Common.SharedKernel.Infrastructure.Serialization;
using Template.Domain.Abstractions.Persistence;
using TickerQ.Utilities.Base;

namespace Template.Infrastructure.Jobs;

public sealed class ProcessOutboxMessagesJob(
    IMediator mediator,
    IApplicationDbContext applicationDbContext,
    ILogger<ProcessOutboxMessagesJob> logger
)
{
    private const int BatchSize = 1000;

    [TickerFunction(nameof(ProcessOutboxMessagesJob), "0 0 * * *")]
    public async Task ProcessAsync()
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
