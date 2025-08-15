using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;

namespace Template.AcceptanceTests.Config;

public class TestSettings
{
    private static readonly Lazy<TestSettings> _instance = new(() => new TestSettings());
    public static TestSettings Instance => _instance.Value;

    private IConfiguration Configuration { get; }

    public string BaseUrl { get; }
    public string ApiBaseUrl { get; }
    public int DefaultTimeout { get; }
    public bool Headless { get; }
    public string Browser { get; }
    public bool UseAspireHost { get; }
    public bool SlowMo { get; }
    public bool RecordVideo { get; }
    public string VideoDir { get; }
    public bool Screenshot { get; }
    public string ScreenshotDir { get; }
    public bool TracingEnabled { get; }
    public string TracingDir { get; }

    private TestSettings()
    {
        string? envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                          ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        Guard.Against.NullOrEmpty(envName);

        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"Config/appsettings.{envName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        BaseUrl = Configuration["TestSettings:BaseUrl"] ?? "http://localhost:3000";
        ApiBaseUrl = Configuration["TestSettings:ApiBaseUrl"] ?? "http://localhost:5000";
        DefaultTimeout = Configuration.GetValue<int>("TestSettings:DefaultTimeout", 30000);
        Headless = Configuration.GetValue<bool>("TestSettings:Headless", true);
        Browser = Configuration["TestSettings:Browser"] ?? "chromium";
        UseAspireHost = Configuration.GetValue<bool>("TestSettings:UseAspireHost", true);
        SlowMo = Configuration.GetValue<bool>("TestSettings:SlowMo", false);
        RecordVideo = Configuration.GetValue<bool>("TestSettings:RecordVideo", false);
        VideoDir = Configuration["TestSettings:VideoDir"] ?? "test-results/videos";
        Screenshot = Configuration.GetValue<bool>("TestSettings:Screenshot", true);
        ScreenshotDir = Configuration["TestSettings:ScreenshotDir"] ?? "test-results/screenshots";
        TracingEnabled = Configuration.GetValue<bool>("TestSettings:TracingEnabled", true);
        TracingDir = Configuration["TestSettings:TracingDir"] ?? "test-results/traces";
    }
}
