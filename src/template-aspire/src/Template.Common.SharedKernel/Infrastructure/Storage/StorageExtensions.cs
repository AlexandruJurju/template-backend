using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Common.SharedKernel.Api;

namespace Template.Common.SharedKernel.Infrastructure.Storage;

public static class StorageExtensions
{
    public static void AddDefaultAzureBlobStorage(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionName
    )
    {
        services.GetRequiredConfiguration<StorageSettings>(StorageSettings.SectionName);

        services.AddSingleton<IBlobStorage, BlobStorage>();

        services.AddSingleton(_ => new BlobServiceClient(configuration.GetRequiredConnectionString(connectionName)));
    }
}
