using Application.Abstractions.Behaviors;
using Domain.Users;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddMediator(options =>
            {
                options.Assemblies = [typeof(DependencyInjection), typeof(UserRegisteredDomainEvent)];
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.NotificationPublisherType = typeof(ForeachAwaitPublisher);
            }
        );
        return services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }
}
