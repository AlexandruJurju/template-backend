using FluentValidation;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Abstractions.Behaviors;
using Template.Domain.Users;

namespace Template.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
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
