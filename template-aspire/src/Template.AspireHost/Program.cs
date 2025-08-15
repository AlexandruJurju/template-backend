using Aspire.Hosting.Azure;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> database = builder
    .AddPostgres("database")
    .WithImage("postgres:17")
    .WithDataVolume()
    .AddDatabase("template");

IResourceBuilder<KeycloakResource> keycloak = builder
    .AddKeycloak(name: "keycloak", 18080)
    .WithDataVolume()
    .WithExternalHttpEndpoints();

IResourceBuilder<GarnetResource> cache = builder.AddGarnet(name: "garnet");

IResourceBuilder<PapercutSmtpContainerResource> papercut = builder.AddPapercutSmtp("papercut");

IResourceBuilder<AzureBlobStorageResource> azureStorage = builder
    .AddAzureStorage("azure-storage")
    .RunAsEmulator()
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
    .WithReference(keycloak)
    .WaitFor(database)
    .WaitFor(cache)
    .WaitFor(papercut)
    .WaitFor(azureStorage)
    .WaitFor(keycloak);

// builder.AddNpmApp("template-ui", "../../../../template-ui")
//     .WithReference(templateApi)
//     .WaitFor(templateApi)
//     .WithHttpEndpoint(env: "PORT", port: 3000, isProxied: false)
//     .WithExternalHttpEndpoints()
//     .PublishAsDockerFile();

await builder.Build().RunAsync();
