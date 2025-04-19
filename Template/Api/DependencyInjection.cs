using Api.Cors;
using Api.ExceptionHandler;

namespace Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();

        AddCors(services, configuration);

        return services;
    }

    private static void AddCors(IServiceCollection services, IConfiguration configuration)
    {
        CorsOptions corsOptions = configuration.GetSection(Cors.CorsOptions.SectionName).Get<CorsOptions>()!;

        services.AddCors(options =>
            options.AddPolicy(CorsOptions.PolicyName, policy =>
                policy
                    .WithOrigins(corsOptions.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader())
        );
    }
}
