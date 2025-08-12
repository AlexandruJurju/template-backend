using Microsoft.Playwright;
using Xunit.Abstractions;

namespace Template.E2E.Tests;

[Collection("AspireCollection")]
public abstract class BaseTest : IAsyncLifetime
{
    protected readonly AspireTestFixture AspireFixture;
    protected readonly ITestOutputHelper Output;
    protected IPlaywright? Playwright;
    protected IBrowser? Browser;
    protected IPage? Page;

    protected BaseTest(AspireTestFixture aspireFixture, ITestOutputHelper output)
    {
        AspireFixture = aspireFixture;
        Output = output;
    }

    public async Task InitializeAsync()
    {
        // Initialize Playwright
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        // Launch browser
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = ["--no-sandbox", "--disable-dev-shm-usage"]
        });

        // Create page
        Page = await Browser.NewPageAsync();

        // Set up console logging
        Page.Console += (_, e) => Output.WriteLine($"Console {e.Type}: {e.Text}");
        Page.PageError += (_, e) => Output.WriteLine($"Page Error: {e}");

        // Reset Aspire state before each test
        await AspireFixture.ResetStateAsync();

        // Log service health status
        Dictionary<string, bool> healthStatus = await AspireFixture.GetServicesHealthAsync();
        foreach ((string service, bool isHealthy) in healthStatus)
        {
            Output.WriteLine($"Service {service}: {(isHealthy ? "Healthy" : "Unhealthy")}");
        }
    }

    public async Task DisposeAsync()
    {
        if (Page is not null)
        {
            // Take screenshot on failure if needed
            try
            {
                await Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = $"screenshot-{GetType().Name}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.png",
                    FullPage = true
                });
            }
            catch
            {
                // Screenshot failed, but don't fail the test cleanup
            }

            await Page.CloseAsync();
        }

        if (Browser is not null)
        {
            await Browser.CloseAsync();
        }

        Playwright?.Dispose();
    }

    protected async Task NavigateToAppAsync()
    {
        if (Page is null || AspireFixture.FrontendHttpClient?.BaseAddress is null)
        {
            throw new InvalidOperationException("Page or Frontend HttpClient not initialized");
        }

        await Page.GotoAsync(AspireFixture.FrontendHttpClient.BaseAddress.ToString());
    }

    protected async Task<IResponse?> WaitForApiCallAsync(string urlPattern, Func<Task> action)
    {
        if (Page is null)
        {
            throw new InvalidOperationException("Page not initialized");
        }

        Task<IResponse> responseTask = Page.WaitForResponseAsync(response =>
            response.Url.Contains(urlPattern) && response.Request.Method != "OPTIONS");

        await action();

        return await responseTask;
    }

    /// <summary>
    /// Ensures the application state is clean before running test logic.
    /// Call this at the beginning of complex tests that modify application state.
    /// </summary>
    protected async Task EnsureCleanStateAsync()
    {
        await AspireFixture.ResetStateAsync();

        // Verify services are healthy before proceeding
        Dictionary<string, bool> healthStatus = await AspireFixture.GetServicesHealthAsync();
        if (healthStatus.Values.Any(isHealthy => !isHealthy))
        {
            IEnumerable<string> unhealthyServices = healthStatus
                .Where(kvp => !kvp.Value)
                .Select(kvp => kvp.Key);

            throw new InvalidOperationException(
                $"Services are not healthy: {string.Join(", ", unhealthyServices)}");
        }
    }

    /// <summary>
    /// Waits for the Angular application to be fully loaded and ready
    /// </summary>
    protected async Task WaitForAngularReadyAsync(TimeSpan? timeout = null)
    {
        if (Page is null)
        {
            throw new InvalidOperationException("Page not initialized");
        }

        int timeoutMs = (int)(timeout?.TotalMilliseconds ?? 15000);

        // Wait for Angular to bootstrap
        await Page.WaitForFunctionAsync(
            "() => window.ng !== undefined || document.readyState === 'complete'",
            new PageWaitForFunctionOptions { Timeout = timeoutMs });

        // Wait for app-root to be visible
        ILocator angularRoot = Page.Locator("app-root, [ng-version]");
        await angularRoot.WaitForAsync(new LocatorWaitForOptions
        {
            Timeout = timeoutMs,
            State = WaitForSelectorState.Visible
        });
    }

    /// <summary>
    /// Helper method to check if UI is responsive using the additional health check
    /// </summary>
    protected async Task<bool> IsUIResponsiveAsync()
    {
        return await AspireFixture.CheckUIHealthAsync(TimeSpan.FromSeconds(5));
    }

    protected Task<IResponse?> WaitForApiCallAsync(Uri urlPattern, Func<Task> action)
    {
        throw new NotImplementedException();
    }
}
