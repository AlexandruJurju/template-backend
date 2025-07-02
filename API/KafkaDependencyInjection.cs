using KafkaFlow;
using KafkaFlow.Serializer;

namespace API;

public static class KafkaDependencyInjection
{
    public static IServiceCollection AddKafkaServices(this IServiceCollection services, IConfiguration configuration)
    {
        const string topicName = "bruh-topic";
        const string producerName = "producer-test";
        string kafkaConnection = configuration.GetConnectionString("kafka")!;

        services.AddKafkaFlowHostedService(kafka => kafka
            .AddCluster(cluster => cluster
                .WithBrokers([kafkaConnection])
                .CreateTopicIfNotExists(topicName, 1, 1)
                .AddProducer(
                    producerName,
                    producer => producer
                        .DefaultTopic(topicName)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                )
                .AddConsumer(consumer => consumer
                    .Topic(topicName)
                    .WithGroupId("test-group")
                    .WithBufferSize(100)
                    .WithWorkersCount(3)
                    .AddMiddlewares(middlewares => middlewares
                        .Add<TestMiddleware>()
                        .AddDeserializer<JsonCoreDeserializer>()
                        .AddTypedHandlers(handlers => handlers
                            .WithHandlerLifetime(InstanceLifetime.Singleton)
                            .AddHandler<HelloMessageHandler>()
                        )
                    )
                )
            )
        );

        return services;
    }
}
