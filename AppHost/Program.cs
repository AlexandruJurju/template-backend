using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> database = builder.AddPostgres("template-postgres")
    .WithBindMount("../.containers/database", "/var/lib/postgresql/data");

IResourceBuilder<RedisResource> cache = builder.AddRedis("template-redis");

IResourceBuilder<PapercutSmtpContainerResource> papercut = builder.AddPapercutSmtp("template-papercut");

IResourceBuilder<ProjectResource> api = builder.AddProject<Template_API>("template-api")
    .WithReference(database)
    .WithReference(cache)
    .WithReference(papercut)
    .WithEnvironment("ConnectionStrings__Database", database)
    .WithEnvironment("ConnectionStrings__Cache", cache)
    .WithEnvironment("ConnectionStrings__Papercut", papercut)
    .WaitFor(database)
    .WaitFor(cache);

builder.AddNpmApp("template-ui", "../../template-ui")
    .WithReference(api)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

await builder.Build().RunAsync();
