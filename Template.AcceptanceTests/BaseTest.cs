using Microsoft.Playwright;
using Template.E2E.Tests;
using Xunit.Abstractions;

namespace Template.AcceptanceTests;

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
    }

    public async Task DisposeAsync()
    {
        if (Page is not null)
        {
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
        if (Page is null || string.IsNullOrEmpty(AspireFixture.FrontendUrl))
        {
            throw new InvalidOperationException("Page or FrontendUrl not initialized");
        }

        await Page.GotoAsync(AspireFixture.FrontendUrl);
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

    protected Task<IResponse?> WaitForApiCallAsync(Uri urlPattern, Func<Task> action)
    {
        throw new NotImplementedException();
    }
}
