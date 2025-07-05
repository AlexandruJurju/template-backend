using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("template-postgres")
    .WithBindMount("../.containers/database", "/var/lib/postgresql/data");

IResourceBuilder<RedisResource> cache = builder.AddRedis("template-redis");

IResourceBuilder<PapercutSmtpContainerResource> papercut = builder.AddPapercutSmtp("template-papercut");

IResourceBuilder<ProjectResource> api = builder.AddProject<Template_API>("template-api")
    .WithReference(postgres)
    .WithReference(cache)
    .WithReference(papercut)
    .WaitFor(postgres)
    .WaitFor(cache);

builder.AddNpmApp("template-ui", "../../template-ui")
    .WithReference(api)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

await builder.Build().RunAsync();
