using Application.Abstractions.Outbox;
using Application.Abstractions.Persistence;
using Domain.Abstractions;
using Domain.Infrastructure.Outbox;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Outbox;

public class ProcessOutboxMessagesJob(
    IMediator mediator,
    IApplicationDbContext applicationDbContext,
    ILogger<ProcessOutboxMessagesJob> logger)
    : IProcessOutboxMessagesJob
{
    private const int BATCH_SIZE = 1000;

    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public async Task ProcessAsync()
    {
        logger.LogInformation("Starting to process outbox messages");

        List<OutboxMessage> outboxMessages = await applicationDbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .Take(BATCH_SIZE)
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
                IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    JsonSerializerSettings)!;

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
            outboxMessage.ProcessedOnUtc = TimeProvider.System.GetUtcNow().UtcDateTime;
            outboxMessage.Error = exception?.ToString();
        }

        await applicationDbContext.SaveChangesAsync();
    }
}
