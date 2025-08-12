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
    private WebApplicationFixture AppFixture { get; set; } = null!;
    private TestSettings Settings { get; set; } = null!;
    private IBrowserContext BrowserContext { get; set; } = null!;
    protected string BaseUrl { get; private set; } = string.Empty;
    protected string ApiUrl { get; private set; } = string.Empty;

    private static readonly string[] Options = ["geolocation", "notifications"];

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

        if (testStatus == TestStatus.Failed && Settings.Screenshot)
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

        if (Settings.TracingEnabled)
        {
            if (testStatus == TestStatus.Failed)
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

                if (testStatus == TestStatus.Failed)
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
            TimezoneId = "Europe/London",
            Permissions = Options,
            ColorScheme = ColorScheme.Dark,
        });

        Page.SetDefaultTimeout(Settings.DefaultTimeout);

        Page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                TestContext.Out.WriteLine($"Console Error: {msg.Text}");
            }
        };

        Page.RequestFailed += (_, request) => TestContext.Out.WriteLine($"Request Failed: {request.Url} - {request.Failure}");
    }

    protected async Task WaitForAngularAsync()
    {
        await WaitHelpers.WaitForAngularAsync(Page);
    }
}
