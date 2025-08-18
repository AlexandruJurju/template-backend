using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Template.Common.SharedKernel.Infrastructure.Configuration;

namespace Template.Common.SharedKernel.Infrastructure.MongoDb;

public static class MongoDbExtensions
{
    public static void AddDefaultMongo(
        this IServiceCollection services,
        IConfiguration configuration,
        string name)
    {
        var connectionString = configuration.GetConnectionStringOrThrow(name);

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

        services.AddScoped<IMongoDatabase>(serviceProvider =>
        {
            IMongoClient client = serviceProvider.GetRequiredService<IMongoClient>();
            var dbName = GetDatabaseNameFromConnectionString(connectionString)
                         ?? throw new InvalidOperationException(
                             $"Database name not provided and could not be extracted from connection string '{name}'");
            return client.GetDatabase(dbName);
        });

        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
    }

    private static string? GetDatabaseNameFromConnectionString(string connectionString)
    {
        try
        {
            var url = new MongoUrl(connectionString);
            return url.DatabaseName;
        }
        catch
        {
            return null;
        }
    }
}
