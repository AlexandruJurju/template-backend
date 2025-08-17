using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Template.Common.SharedKernel.Infrastructure.Authentication.Jwt;

[OptionsValidator]
public sealed partial class JwtOptions : IValidateOptions<JwtOptions>
{
    internal const string SectionName = "Jwt";
    [Required] public string Secret { get; init; }
    [Required] public string Issuer { get; init; }
    [Required] public string Audience { get; init; }
    [Required] public int ExpireInMinutes { get; init; }
    [Required] public int RefreshTokenExpireInMinutes { get; init; }
}
