using System.Text.Json;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework.Interfaces;
using Template.AcceptanceTests.Config;
using Template.AcceptanceTests.Fixtures;
using Template.AcceptanceTests.Utilities;

namespace Template.AcceptanceTests.Tests;

[TestFixture]
internal abstract class BaseTest : PageTest
{
    protected WebApplicationFixture AppFixture { get; private set; } = null!;
    protected TestSettings Settings { get; private set; } = null!;
    protected string BaseUrl { get; private set; } = string.Empty;
    protected string ApiUrl { get; private set; } = string.Empty;
    protected IBrowserContext BrowserContext { get; private set; } = null!;
    protected ITracing? Tracing { get; private set; }

    private static readonly string[] options = new[] { "geolocation", "notifications" };

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Settings = TestSettings.Instance;
        AppFixture = new WebApplicationFixture();
        await AppFixture.InitializeAsync();

        BaseUrl = AppFixture.BaseUrl;
        ApiUrl = AppFixture.ApiUrl;
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await AppFixture.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        await CreateContextAsync();

        if (Settings.TracingEnabled)
        {
            await BrowserContext.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }
    }

    [TearDown]
    public async Task TearDown()
    {
        string testName = TestContext.CurrentContext.Test.Name;
        TestStatus testStatus = TestContext.CurrentContext.Result.Outcome.Status;

        // Take screenshot on failure
        // if (testStatus == TestStatus.Failed && Settings.Screenshot)

        if (Settings.Screenshot)
        {
            string screenshotPath = Path.Combine(
                Settings.ScreenshotDir,
                $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            );

            Directory.CreateDirectory(Settings.ScreenshotDir);
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });

            TestContext.AddTestAttachment(screenshotPath, "Failure Screenshot");
        }

        // Save trace on failure
        if (Settings.TracingEnabled)
        {
            if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                string tracePath = Path.Combine(
                    Settings.TracingDir,
                    $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.zip"
                );

                Directory.CreateDirectory(Settings.TracingDir);
                await BrowserContext.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
                TestContext.AddTestAttachment(tracePath, "Trace");
            }
            else
            {
                await BrowserContext.Tracing.StopAsync();
            }
        }

        // Save video if enabled
        if (Settings.RecordVideo && Page.Video != null)
        {
            string videoPath = await Page.Video.PathAsync();
            if (!string.IsNullOrEmpty(videoPath) && File.Exists(videoPath))
            {
                string finalVideoPath = Path.Combine(
                    Settings.VideoDir,
                    $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.webm"
                );

                Directory.CreateDirectory(Settings.VideoDir);
                File.Move(videoPath, finalVideoPath, true);

                if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    TestContext.AddTestAttachment(finalVideoPath, "Test Video");
                }
            }
        }
    }

    private async Task CreateContextAsync()
    {
        var browserOptions = new BrowserTypeLaunchOptions
        {
            Headless = Settings.Headless,
            SlowMo = Settings.SlowMo ? 100 : 0,
            Args = ["--start-maximized"]
        };

#pragma warning disable CA1304
#pragma warning disable CA1311
        IBrowser browser = Settings.Browser.ToLower() switch
#pragma warning restore CA1311
#pragma warning restore CA1304
        {
            "firefox" => await Playwright.Firefox.LaunchAsync(browserOptions),
            "webkit" => await Playwright.Webkit.LaunchAsync(browserOptions),
            _ => await Playwright.Chromium.LaunchAsync(browserOptions)
        };

        BrowserContext = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = BaseUrl,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            IgnoreHTTPSErrors = true,
            RecordVideoDir = Settings.RecordVideo ? Settings.VideoDir : null,
            RecordVideoSize = Settings.RecordVideo ? new RecordVideoSize { Width = 1920, Height = 1080 } : null,
            Locale = "en-US",
            TimezoneId = "America/New_York",
            Permissions = options,
            ColorScheme = ColorScheme.Light
        });

        Page.SetDefaultTimeout(Settings.DefaultTimeout);

        // Log console messages for debugging
        Page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                TestContext.Out.WriteLine($"Console Error: {msg.Text}");
            }
        };

        // Log network failures
        Page.RequestFailed += (_, request) => TestContext.Out.WriteLine($"Request Failed: {request.Url} - {request.Failure}");
    }

    protected async Task WaitForAngularAsync()
    {
        await WaitHelpers.WaitForAngularAsync(Page);
    }

    protected async Task<IAPIResponse> CallApiAsync(string endpoint, HttpMethod method, object? data = null)
    {
        string url = $"{ApiUrl}/{endpoint.TrimStart('/')}";

        return method.Method switch
        {
            "GET" => await Page.APIRequest.GetAsync(url),
            "POST" => await Page.APIRequest.PostAsync(url, new APIRequestContextOptions { DataObject = data }),
            "PUT" => await Page.APIRequest.PutAsync(url, new APIRequestContextOptions { DataObject = data }),
            "DELETE" => await Page.APIRequest.DeleteAsync(url),
            _ => throw new NotSupportedException($"HTTP method {method} is not supported")
        };
    }

    protected async Task<T> CallApiAsync<T>(string endpoint, HttpMethod method, object? data = null)
    {
        IAPIResponse response = await CallApiAsync(endpoint, method, data);
        JsonElement? json = await response.JsonAsync();
        return json!.Value.Deserialize<T>()!;
    }
}
