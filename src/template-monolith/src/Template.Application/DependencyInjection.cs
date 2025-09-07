using Template.Application.Contracts.Services;
using Template.Application.Features.Users;
using Template.Application.Services;
using Template.Common.Constants.Aspire;
using Template.Common.SharedKernel.Application.CQRS.Mediator;
using Template.Common.SharedKernel.Application.EventBus;
using Template.Common.SharedKernel.Application.Mapper;

namespace Template.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultMediatR<ApplicationAssemblyMarker>();

        services.AddSignalR();

        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly, includeInternalTypes: true);

        services.AddMappersFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

        services.AddMassTransitEventBus(configuration, typeof(ApplicationAssemblyMarker));

        AddBackgroundServices(services);

        AddServices(services);
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IRandomNumberService, RandomNumberService>();
    }

    private static void AddBackgroundServices(IServiceCollection services)
    {
        // services.AddHostedService<RandomNumberBackgroundService>();
    }
}
