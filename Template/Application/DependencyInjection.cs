using Application.Abstractions.Behaviors;
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
                options.Assemblies = [typeof(DependencyInjection)];
                options.ServiceLifetime = ServiceLifetime.Scoped;
            }
        );
        return services
            .AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>))
            .AddSingleton(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }
}
