using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Template.Common.SharedKernel.Infrastructure.Configuration;

public static class ConfigurationExtensions
{
    public static void AddOptionsWithValidation<TSetting>(
        this IServiceCollection services,
        string section,
        string? name = null,
        Action<TSetting>? configure = null
    )
        where TSetting : class, new()
    {
        services
            .AddOptionsWithValidateOnStart<TSetting>(name)
            .Configure(options => configure?.Invoke(options))
            .BindConfiguration(section)
            .ValidateDataAnnotations();

        services.TryAddSingleton(sp =>
        {
            IOptions<TSetting> options = sp.GetRequiredService<IOptions<TSetting>>();
            TSetting setting = options.Value;
            return setting;
        });
    }

    public static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
    {
        var connectionString = configuration.GetConnectionString(name);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}"
            );
        }

        return connectionString;
    }

    public static T GetValueOrThrow<T>(this IConfiguration configuration, string name)
    {
        return configuration.GetValue<T?>(name) ??
               throw new InvalidOperationException($"The connection string {name} was not found");
    }
}
