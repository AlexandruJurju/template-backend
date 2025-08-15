using Template.Application.BackgroundServices;
using Template.Application.Contracts.Services;
using Template.Application.Services;
using Template.SharedKernel.Application.Behaviors;

namespace Template.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        AddMediatR(services);

        services.AddSignalR();

        AddBackgroundServices(services);

        AddServices(services);

        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IRandomNumberService, RandomNumberService>();
    }

    private static void AddBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<RandomNumberBackgroundService>();
    }

    private static void AddMediatR(IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(QueryCachingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
    }
}
