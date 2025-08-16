using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Template.Common.SharedKernel.Infrastructure.EF;

public static class DbContextExtensions
{
    public static void AddPostgresDbContext<TDbContext>(
        this IHostApplicationBuilder builder,
        string name,
        Action<IHostApplicationBuilder>? action = null
    )
        where TDbContext : DbContext
    {
        IServiceCollection services = builder.Services;

        services.AddSingleton<ISaveChangesInterceptor, InsertOutboxMessagesInterceptor>();

        services.AddDbContext<TDbContext>((sp, options) =>
            {
                options
                    .UseNpgsql(builder.Configuration.GetConnectionString(name))
                    .UseSnakeCaseNamingConvention()
                    // Issue: https://github.com/dotnet/efcore/issues/35285
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(RelationalEventId.PendingModelChangesWarning)
                    );

                IInterceptor[] interceptors = [.. sp.GetServices<IInterceptor>()];

                if (interceptors.Length != 0)
                {
                    options.AddInterceptors(interceptors);
                }
            }
        );

        action?.Invoke(builder);
    }
}
