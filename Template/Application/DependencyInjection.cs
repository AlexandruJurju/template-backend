using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.FluentValidation;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddWolverine();
        return services;
    }

    private static IServiceCollection AddWolverine(this IServiceCollection services)
    {
        services.AddWolverine(options =>
        {
            // Apply the validation middleware *and* discover and register Fluent Validation validators
            options.UseFluentValidation();

            options.Discovery.IncludeAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}