using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Common.SharedKernel.Api;
using Template.Common.SharedKernel.Infrastructure.Configuration;

namespace Template.Common.SharedKernel.Infrastructure.Dapper;

public static class DapperExtensions
{
    public static void AddDefaultDapper(
        this IServiceCollection services,
        IConfiguration configuration,
        string name)
    {
        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(configuration.GetConnectionStringOrThrow(name))
        );
    }
}
