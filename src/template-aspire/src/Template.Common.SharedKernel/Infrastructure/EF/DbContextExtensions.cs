using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Common.SharedKernel.Infrastructure.Configuration;
using Template.Common.SharedKernel.Infrastructure.Dapper;
using Template.Common.SharedKernel.Infrastructure.Repository;

namespace Template.Common.SharedKernel.Infrastructure.EF;

public static class DbContextExtensions
{
    public static void AddDefaultPostgresDb<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string name
    )
        where TDbContext : DbContext, IUnitOfWork
    {
        services.AddSingleton<IInterceptor, InsertOutboxMessagesInterceptor>();

        services.AddDbContext<TDbContext>((sp, options) =>
            {
                options
                    .UseNpgsql(configuration.GetConnectionStringOrThrow(name))
                    .UseSnakeCaseNamingConvention()
                    // Issue: https://github.com/dotnet/efcore/issues/35285
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(RelationalEventId.PendingModelChangesWarning)
                    );

                IInterceptor[] interceptors = sp.GetServices<IInterceptor>().ToArray();

                if (interceptors.Length != 0)
                {
                    options.AddInterceptors(interceptors);
                }
            }
        );

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<TDbContext>());

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(configuration.GetConnectionStringOrThrow(name))
        );
    }
}
