using Aspire.Hosting.Azure;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> database = builder
    .AddPostgres("database")
    .WithImage("postgres:17")
    .WithBindMount("../../../.containers/db", "/var/lib/postgresql/data")
    .AddDatabase("template");

// IResourceBuilder<RedisResource> cache = builder.AddRedis("template-redis");

IResourceBuilder<GarnetResource> cache = builder.AddGarnet("template-garnet");

IResourceBuilder<PapercutSmtpContainerResource> papercut = builder.AddPapercutSmtp("template-papercut");

IResourceBuilder<AzureBlobStorageResource> azureStorage = builder
    .AddAzureStorage("azure-storage")
    .RunAsEmulator(azurite =>
        azurite.WithDataBindMount("../../../.containers/blob_storage/data"))
    .AddBlobs("blob-storage");

builder.AddProject<Template_API>("template-api")
    .WithEnvironment("ConnectionStrings__Database", database)
    .WithEnvironment("ConnectionStrings__Cache", cache)
    .WithEnvironment("ConnectionStrings__Papercut", papercut)
    .WithEnvironment("ConnectionStrings__AzureStorage", azureStorage)
    .WithReference(database)
    .WithReference(cache)
    .WithReference(papercut)
    .WithReference(azureStorage)
    .WaitFor(database)
    .WaitFor(cache)
    .WaitFor(papercut)
    .WaitFor(azureStorage);

// builder.AddNpmApp("template-ui", "../../template-ui")
//     .WithReference(api)
//     .WithExternalHttpEndpoints()
//     .PublishAsDockerFile();

await builder.Build().RunAsync();
