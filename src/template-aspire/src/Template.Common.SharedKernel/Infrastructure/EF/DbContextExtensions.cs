using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Template.Common.SharedKernel.Api;
using Template.Common.SharedKernel.Infrastructure.Repository;

namespace Template.Common.SharedKernel.Infrastructure.EF;

public static class DbContextExtensions
{
    public static void AddDefaultPostgresDb<TDbContext>(
        this IHostApplicationBuilder builder,
        string name,
        Action<IHostApplicationBuilder>? action = null
    )
        where TDbContext : DbContext
    {
        IServiceCollection services = builder.Services;

        services.AddSingleton<IInterceptor, InsertOutboxMessagesInterceptor>();

        services.AddDbContext<TDbContext>((sp, options) =>
            {
                options
                    .UseNpgsql(builder.Configuration.GetRequiredConnectionString(name))
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

        services.AddScoped<IUnitOfWork, EfUnitOfWork<TDbContext>>();

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        action?.Invoke(builder);
    }
}
