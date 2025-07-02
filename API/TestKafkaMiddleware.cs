using KafkaFlow;

namespace API;

public class TestMiddleware : IMessageMiddleware
{
    private readonly ILogger<TestMiddleware> _logger;

    public TestMiddleware(ILogger<TestMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        _logger.LogInformation("TestMiddleware: Before processing message {MessageType}", context.Message.GetType().Name);

        // Call next middleware or handler
        await next(context);

        _logger.LogInformation("TestMiddleware: After processing message {MessageType}", context.Message.GetType().Name);
    }
}
