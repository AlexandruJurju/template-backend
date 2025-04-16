using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.FluentValidation;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddFluentValidation(services);
        
        AddWolverine(services);

        return services;
    }

    private static void AddFluentValidation(IServiceCollection services)
    {
        // Register all validators in the assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
    }

    private static void AddWolverine(IServiceCollection services)
    {
        services.AddWolverine(options =>
        {
            options.UseFluentValidation();

            options.Discovery.IncludeAssembly(typeof(DependencyInjection).Assembly);
        });
    }
}
