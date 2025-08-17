using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Template.Application.Hubs;
using Template.Common.SharedKernel.Api.Cors;
using Template.Common.SharedKernel.Api.Exceptions;

namespace Template.API;

public static class DependencyInjection
{
    public static void AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        AddSwaggerGenWithAuth(services);

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

    private static void AddSwaggerGenWithAuth(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            // SignalR support
            options.AddSignalRSwaggerGen(signalROptions =>
                // Scan the assembly containing the hubs
                signalROptions.ScanAssembly(typeof(RandomNumberHub).Assembly));

            // Fix problem with swagger and scalar ui -> treats strings as nullable even if they are not marked as nullable
            options.SupportNonNullableReferenceTypes();

            options.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            // Fix conflict for api versions
            options.ResolveConflictingActions(descriptions => descriptions.First());

            // Add xml comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            // ================== Keycloak Configuration ==================
            // var keycloakSecurityScheme = new OpenApiSecurityScheme
            // {
            //     Type = SecuritySchemeType.OAuth2,
            //     Flows = new OpenApiOAuthFlows
            //     {
            //         Implicit = new OpenApiOAuthFlow
            //         {
            //             AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"]!),
            //             Scopes = new Dictionary<string, string>
            //             {
            //                 { "openid", "openid" },
            //                 { "profile", "profile" }
            //             }
            //         }
            //     }
            // };
            //
            // options.AddSecurityDefinition("Keycloak", keycloakSecurityScheme);
            //
            // options.AddSecurityRequirement(new OpenApiSecurityRequirement
            // {
            //     {
            //         new OpenApiSecurityScheme
            //         {
            //             Reference = new OpenApiReference
            //             {
            //                 Id = "Keycloak",
            //                 Type = ReferenceType.SecurityScheme
            //             },
            //             In = ParameterLocation.Header,
            //             Name = "Bearer",
            //             Scheme = "Bearer"
            //         },
            //         []
            //     }
            // });

            // ================== JWT Configuration ==================
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter your JWT token in this field",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    []
                }
            });
        });
    }
}
