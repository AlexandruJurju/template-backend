using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Common.SharedKernel.Api;

namespace Template.Common.SharedKernel.Infrastructure.Dapper;

public static class DapperExtensions
{
    public static void AddDefaultDapper(
        this IServiceCollection services,
        IConfiguration configuration,
        string name)
    {
        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(configuration.GetRequiredConnectionString(name))
        );
    }
}
