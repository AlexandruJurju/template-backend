using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Template.Common.SharedKernel.Infrastructure.Configuration;
using Template.Common.SharedKernel.Infrastructure.Persistence.Abstractions;
using Template.Common.SharedKernel.Infrastructure.Persistence.Dapper;
using Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework.Repository;

namespace Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework;

public static class DbContextExtensions
{
    public static void AddDefaultPostgresDb<TDbContext>(
        this IHostApplicationBuilder builder,
        string name,
        Action<IHostApplicationBuilder>? action = null
    )
        where TDbContext : DbContext, IUnitOfWork
    {
        IServiceCollection services = builder.Services;

        services.AddSingleton<IInterceptor, InsertOutboxMessagesInterceptor>();

        services.AddDbContext<TDbContext>((sp, options) =>
            {
                options
                    .UseNpgsql(builder.Configuration.GetConnectionString(name))
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
            new SqlConnectionFactory(builder.Configuration.GetConnectionStringOrThrow(name))
        );

        action?.Invoke(builder);
    }
}
