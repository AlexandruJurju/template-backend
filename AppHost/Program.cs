using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("template-postgres")
    .WithBindMount("../.containers/database", "/var/lib/postgresql/data");

IResourceBuilder<RedisResource> cache = builder.AddRedis("template-redis");

// var mongo = builder.AddMongoDB("template-mongo")
//     .WithBindMount("../.containers/database", "/var/lib/mongodb/data");


IResourceBuilder<PapercutSmtpContainerResource> papercut = builder.AddPapercutSmtp("template-papercut");

builder.AddProject<Template_API>("template-api")
    .WithReference(postgres)
    .WithReference(cache)
    .WithReference(papercut)
    .WaitFor(postgres)
    .WaitFor(cache);

await builder.Build().RunAsync();
