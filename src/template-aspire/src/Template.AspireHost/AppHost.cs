using Aspire.Hosting.Azure;
using Projects;
using Scalar.Aspire;
using Template.Common.Constants.Aspire;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgresServer = builder
    .AddPostgres(Components.Postgres)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<PostgresDatabaseResource> templatePostgresDb = postgresServer
    .AddDatabase(Components.RelationalDbs.Template);

// IResourceBuilder<KeycloakResource> keycloak = builder
//     .AddKeycloak(Components.KeyCloak, 18080)
//     .WithDataVolume()
//     .WithExternalHttpEndpoints()
//     .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<SeqResource> seq = builder
    .AddSeq(Components.Seq, 5341)
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<RedisResource> redis = builder
    .AddRedis(Components.Redis)
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<MailPitContainerResource> mailpit = builder
    .AddMailPit(Components.MailPit)
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<AzureStorageResource> storage = builder
    .AddAzureStorage(Components.Azure.Storage)
    .RunAsEmulator(config =>
        config
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent)
    );

IResourceBuilder<MongoDBServerResource> mongoServer = builder
    .AddMongoDB(Components.MongoDb)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<MongoDBDatabaseResource> templateMongoDb = mongoServer
    .AddDatabase(Components.DocumentDbs.Template);

IResourceBuilder<RabbitMQServerResource> rabbitMq = builder
    .AddRabbitMQ(Components.RabbitMq)
    .WithManagementPlugin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<AzureBlobStorageContainerResource> blobStorage = storage
    .AddBlobContainer(Components.Azure.BlobContainer);

builder.AddProject<Template_API>(Services.TemplateApi)
    .WithReference(templatePostgresDb).WaitFor(templatePostgresDb)
    .WithReference(redis).WaitFor(redis)
    .WithReference(mailpit).WaitFor(mailpit)
    .WithReference(blobStorage).WaitFor(blobStorage)
    .WithReference(seq).WaitFor(seq)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(templateMongoDb).WaitFor(templateMongoDb);

// Add Scalar API Reference for all services
// builder
//     .AddScalarApiReference()
//     .WithApiReference(templateService, options => options.WithOpenApiRoutePattern("/swagger/v1/swagger.json"))
//     .WaitFor(templateService);

// builder.AddNpmApp(Services.AngularUi, "../../../../template-ui")
//     .WithReference(templateApi)
//     .WaitFor(templateApi)
//     .WithHttpEndpoint(env: "PORT", port: 3000, isProxied: false)
//     .WithExternalHttpEndpoints()
//     .PublishAsDockerFile();

await builder.Build().RunAsync();
