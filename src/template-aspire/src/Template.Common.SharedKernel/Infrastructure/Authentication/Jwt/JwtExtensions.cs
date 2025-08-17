using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Template.Common.SharedKernel.Infrastructure.Configuration;

namespace Template.Common.SharedKernel.Infrastructure.Authentication.Jwt;

public static class JwtExtensions
{
    public static void AddDefaultJwtAuthentication(this IServiceCollection services)
    {
        services.AddOptionsWithValidation<Authentication.Jwt.JwtOptions>(Authentication.Jwt.JwtOptions.SectionName);

        Authentication.Jwt.JwtOptions options = services.BuildServiceProvider().GetRequiredService<Authentication.Jwt.JwtOptions>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret)),
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
    }
}
