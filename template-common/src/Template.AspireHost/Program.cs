using Aspire.Hosting.Azure;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> database = builder
    .AddPostgres("database")
    .WithImage("postgres:17")
    // .WithBindMount("../../../.containers/db", "/var/lib/postgresql/data")
    .AddDatabase("template");

// IResourceBuilder<RedisResource> cache = builder.AddRedis("template-redis");

IResourceBuilder<GarnetResource> cache = builder.AddGarnet("garnet");

IResourceBuilder<PapercutSmtpContainerResource> papercut = builder.AddPapercutSmtp("papercut");

IResourceBuilder<AzureBlobStorageResource> azureStorage = builder
    .AddAzureStorage("azure-storage")
    .RunAsEmulator()
    .AddBlobs("blob-storage");

IResourceBuilder<ProjectResource> templateApi = builder.AddProject<Template_API>("template-api")
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

builder.AddNpmApp("template-ui", "../../../../template-ui")
    .WithReference(templateApi)
    .WaitFor(templateApi)
    .WithHttpEndpoint(env: "PORT", port: 3000, isProxied: false)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

await builder.Build().RunAsync();
