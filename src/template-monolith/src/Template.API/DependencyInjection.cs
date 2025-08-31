using Template.Common.SharedKernel.Api.Cors;
using Template.Common.SharedKernel.Api.Exceptions;
using Template.Common.SharedKernel.Api.Swagger;

namespace Template.API;

public static class DependencyInjection
{
    public static void AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddCommonSwagger();

        AddCors(services, configuration);
    }

    private static void AddCors(IServiceCollection services, IConfiguration configuration)
    {
        CorsOptions corsOptions = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>()!;

        services.AddCors(options => options.AddPolicy(CorsOptions.PolicyName, policy =>
            policy
                .WithOrigins(corsOptions.AllowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => true)));
    }
}
