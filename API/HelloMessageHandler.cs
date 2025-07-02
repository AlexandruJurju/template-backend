using KafkaFlow;

namespace API;

public class HelloMessageHandler(
    ILogger<HelloMessageHandler> logger
) : IMessageHandler<HelloMessage>
{
    public Task Handle(IMessageContext context, HelloMessage message)
    {
        logger.LogInformation("Id {Id}", message.Id);

        return Task.CompletedTask;
    }
}
