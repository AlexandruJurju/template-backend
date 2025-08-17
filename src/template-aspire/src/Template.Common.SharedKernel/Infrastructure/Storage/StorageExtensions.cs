using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Common.SharedKernel.Api;
using Template.Common.SharedKernel.Infrastructure.Configuration;

namespace Template.Common.SharedKernel.Infrastructure.Storage;

public static class StorageExtensions
{
    public static void AddDefaultAzureBlobStorage(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionName
    )
    {
        services.AddOptionsWithValidation<StorageOptions>(StorageOptions.SectionName);

        services.AddSingleton<IBlobStorage, BlobStorage>();

        services.AddSingleton(_ => new BlobServiceClient(configuration.GetConnectionStringOrThrow(connectionName)));
    }
}
