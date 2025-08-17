using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Template.Common.Constants.Aspire;
using Template.Common.SharedKernel.Infrastructure.Configuration;

namespace Template.Common.SharedKernel.Application.EventBus;

public static class EventBusExtensions
{
    public static void AddMassTransitEventBus(
        this IServiceCollection services,
        IConfiguration configuration,
        Type type,
        Action<IBusRegistrationConfigurator>? busConfigure = null)
    {
        services.AddScoped<IEventBus, EventBus>();

        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.AddConsumers(type.Assembly);

            config.AddActivities(type.Assembly);

            config.AddRequestClient(type);

            config.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(configuration.GetConnectionStringOrThrow(Components.RabbitMq)));
                    configurator.ConfigureEndpoints(context);
                    configurator.UseMessageRetry(options => options.Exponential(
                            3,
                            TimeSpan.FromMilliseconds(200),
                            TimeSpan.FromMinutes(120),
                            TimeSpan.FromMilliseconds(200)
                        )
                        .Ignore<ValidationException>()
                    );
                }
            );

            busConfigure?.Invoke(config);
        });
    }
}
