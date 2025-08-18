﻿namespace Template.Common.SharedKernel.Api.Cors;

public sealed class CorsOptions
{
    public const string PolicyName = "TemplateCorsPolicy";
    public const string SectionName = "Cors";

    public required string[] AllowedOrigins { get; init; }
}
