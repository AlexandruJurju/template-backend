using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Template.Common.SharedKernel.Api;

namespace Template.Common.SharedKernel.Infrastructure.Auth.Jwt;

public static class JwtExtensions
{
    public static void AddDefaultJwtAuthentication(this IServiceCollection services)
    {
        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddKeycloakJwtBearer(
        //         Components.KeyCloak,
        //         "template-realm",
        //         options =>
        //         {
        //             options.RequireHttpsMetadata = false;
        //             options.Audience = "account";
        //         }
        //     );

        services.GetRequiredConfiguration<JwtSettings>(JwtSettings.SectionName);

        JwtSettings settings = services.BuildServiceProvider().GetRequiredService<JwtSettings>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
    }
}
