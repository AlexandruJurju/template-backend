using Aspire.Hosting.Azure;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> database = builder
    .AddPostgres("database")
    .WithImage("postgres:17")
    .WithDataVolume()
    .AddDatabase("template");

IResourceBuilder<KeycloakResource> keycloak = builder
    .AddKeycloak("keycloak", 18080)
    .WithDataVolume()
    .WithExternalHttpEndpoints();

IResourceBuilder<GarnetResource> cache = builder.AddGarnet("garnet");

IResourceBuilder<MailPitContainerResource> mailpit = builder.AddMailPit("mailpit");

IResourceBuilder<AzureBlobStorageResource> azureStorage = builder
    .AddAzureStorage("azure-storage")
    .RunAsEmulator()
    .AddBlobs("blob-storage");

builder.AddProject<Template_API>("template-api")
    .WithEnvironment("ConnectionStrings__Database", database)
    .WithEnvironment("ConnectionStrings__Cache", cache)
    .WithEnvironment("ConnectionStrings__Mailpit", mailpit)
    .WithEnvironment("ConnectionStrings__AzureStorage", azureStorage)
    .WithReference(database)
    .WithReference(cache)
    .WithReference(mailpit)
    .WithReference(azureStorage)
    .WithReference(keycloak)
    .WaitFor(database)
    .WaitFor(cache)
    .WaitFor(mailpit)
    .WaitFor(azureStorage)
    .WaitFor(keycloak);

// builder.AddNpmApp("template-ui", "../../../../template-ui")
//     .WithReference(templateApi)
//     .WaitFor(templateApi)
//     .WithHttpEndpoint(env: "PORT", port: 3000, isProxied: false)
//     .WithExternalHttpEndpoints()
//     .PublishAsDockerFile();

await builder.Build().RunAsync();
