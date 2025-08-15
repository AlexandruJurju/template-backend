using Aspire.Hosting.Azure;
using Projects;
using Scalar.Aspire;
using Template.Constants.Aspire;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgresServer = builder
    .AddPostgres(Components.Postgres)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<PostgresDatabaseResource> templateDb = postgresServer
    .AddDatabase(Components.Database.Template);

IResourceBuilder<KeycloakResource> keycloak = builder
    .AddKeycloak(Components.KeyCloak, 18080)
    .WithDataVolume()
    .WithExternalHttpEndpoints()
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<GarnetResource> cache = builder.AddGarnet(Components.Cache);

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

IResourceBuilder<AzureBlobStorageContainerResource> blobStorage = storage
    .AddBlobContainer(Components.Azure.BlobContainer);

IResourceBuilder<ProjectResource> templateService = builder.AddProject<Template_API>(Services.MonolithApi)
    .WithReference(templateDb)
    .WaitFor(templateDb)
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(mailpit)
    .WaitFor(mailpit)
    .WithReference(blobStorage)
    .WaitFor(blobStorage)
    .WithReference(keycloak)
    .WaitFor(keycloak);

// Add Scalar API Reference for all services
builder
    .AddScalarApiReference()
    .WithApiReference(templateService, options => options.WithOpenApiRoutePattern("/swagger/v1/swagger.json"))
    .WaitFor(templateService);

// builder.AddNpmApp(Services.AngularUi, "../../../../template-ui")
//     .WithReference(templateApi)
//     .WaitFor(templateApi)
//     .WithHttpEndpoint(env: "PORT", port: 3000, isProxied: false)
//     .WithExternalHttpEndpoints()
//     .PublishAsDockerFile();

await builder.Build().RunAsync();
