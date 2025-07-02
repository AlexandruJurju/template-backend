IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("template-postgres")
    .WithBindMount("../.containers/database", "/var/lib/postgresql/data");

IResourceBuilder<RedisResource> cache = builder.AddRedis("template-redis");

// var mongo = builder.AddMongoDB("template-mongo")
//     .WithBindMount("../.containers/database", "/var/lib/mongodb/data");


IResourceBuilder<KafkaServerResource> kafka = builder.AddKafka("kafka")
    .WithKafkaUI()
    .WithBindMount("../.containers/kafka", "/var/lib/kafka");

IResourceBuilder<PapercutSmtpContainerResource> papercut = builder.AddPapercutSmtp("template-papercut");

builder.AddProject<Projects.API>("template-api")
    .WithReference(postgres)
    .WithReference(cache)
    .WithReference(papercut)
    .WithReference(kafka)
    .WaitFor(postgres)
    .WaitFor(cache)
    .WaitFor(papercut)
    .WaitFor(kafka);

await builder.Build().RunAsync();
