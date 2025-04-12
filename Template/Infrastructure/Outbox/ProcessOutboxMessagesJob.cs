using Application.Abstractions.Infrastructure;
using Application.Abstractions.Outbox;
using Application.Abstractions.Persistence;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wolverine;

namespace Infrastructure.Outbox;

public class ProcessOutboxMessagesJob(
    IDateTimeProvider dateTimeProvider,
    IApplicationDbContext applicationDbContext,
    IMessageBus messageBus,
    ILogger<ProcessOutboxMessagesJob> logger)
    : IProcessOutboxMessagesJob
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private const int BATCH_SIZE = 1000;

    public async Task ProcessAsync()
    {
        logger.LogInformation("Starting to process outbox messages");

        // Use a transaction scope for atomicity
        await using var transaction = await applicationDbContext.Database.BeginTransactionAsync();

        var outboxMessages = await applicationDbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .Take(BATCH_SIZE)
            .OrderBy(m => m.OccurredOnUtc)
            .ToListAsync();

        if (outboxMessages.Count == 0)
        {
            logger.LogInformation("No outbox messages to process");
            return;
        }

        try
        {
            foreach (var outboxMessage in outboxMessages)
            {
                Exception? exception = null;

                try
                {
                    var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                        outboxMessage.Content,
                        JsonSerializerSettings)!;

                    await messageBus.InvokeAsync(domainEvent);
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
                outboxMessage.ProcessedOnUtc = dateTimeProvider.UtcNow;
                outboxMessage.Error = exception?.ToString();
            }

            await applicationDbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}