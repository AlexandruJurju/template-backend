using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Template.Common.SharedKernel.Api.Swagger;

public static class SwaggerExtensions
{
    public static void AddCommonSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
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

    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
