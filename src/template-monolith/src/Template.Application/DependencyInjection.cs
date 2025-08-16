using Template.Application.BackgroundServices;
using Template.Application.Contracts.Services;
using Template.Application.Services;
using Template.Common.SharedKernel.Application.CQRS.Mediator;

namespace Template.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddDefaultMediatR<AssemblyMarker>();

        services.AddSignalR();

        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly, includeInternalTypes: true);

        AddBackgroundServices(services);

        AddServices(services);
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IRandomNumberService, RandomNumberService>();
    }

    private static void AddBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<RandomNumberBackgroundService>();
    }
}
