using Microsoft.Playwright;
using Template.AcceptanceTests.Utilities;

namespace Template.AcceptanceTests.PageObjects;

public class HomePage : BasePage
{
    // Selectors
    private const string AppRoot = "app-root";
    private const string NavigationMenu = "nav, .navbar, header nav";
    private const string MainContent = "main, .main-content, .main, router-outlet";
    private const string LoadingSpinner = ".spinner, .loading, .loader";
    private const string GoToTestButton = "button:has-text('Go to TEST'), button:text('Go to TEST')";
    private const string AllButtons = "button";

#pragma warning disable CA1054
    public HomePage(IPage page, string baseUrl) : base(page, baseUrl)
#pragma warning restore CA1054
    {
    }

    public async Task<bool> IsPageLoadedAsync()
    {
        // Wait for Angular app to be ready
        await WaitHelpers.WaitForAngularAsync(Page);

        // Check if app root exists
        return await IsElementVisibleAsync(AppRoot);
    }

    public async Task<bool> IsNavigationVisibleAsync()
    {
        return await IsElementVisibleAsync(NavigationMenu);
    }

    public async Task<string> GetPageTitleAsync()
    {
        return await Page.TitleAsync();
    }

    public async Task<bool> WaitForContentToLoadAsync(int timeoutMs = 10000)
    {
        // Wait for loading spinner to disappear if it exists
        bool hasSpinner = await IsElementVisibleAsync(LoadingSpinner, 1000);
        if (hasSpinner)
        {
            await Page.WaitForSelectorAsync(LoadingSpinner, new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Hidden,
                Timeout = timeoutMs
            });
        }

        // Wait for main content to appear
        return await IsElementVisibleAsync(MainContent, timeoutMs);
    }

    public async Task<bool> IsGoToTestButtonVisibleAsync()
    {
        return await IsElementVisibleAsync(GoToTestButton);
    }

    public async Task<TestPage> ClickGoToTestButtonAsync()
    {
        await Page.ClickAsync(GoToTestButton);
        await Page.WaitForURLAsync("**/test");
        await WaitHelpers.WaitForAngularAsync(Page);
        return new TestPage(Page, BaseUrl);
    }

    public async Task<IReadOnlyList<IElementHandle>> GetAllButtonsAsync()
    {
        return await Page.QuerySelectorAllAsync(AllButtons);
    }

    public async Task<List<string>> GetButtonTextsAsync()
    {
        IReadOnlyList<IElementHandle> buttons = await GetAllButtonsAsync();
        var buttonTexts = new List<string>();

        foreach (IElementHandle button in buttons)
        {
            string? text = await button.TextContentAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                buttonTexts.Add(text.Trim());
            }
        }

        return buttonTexts;
    }

    public async Task<bool> HasConsoleErrorsAsync()
    {
        var errors = new List<string>();
        Page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                errors.Add(msg.Text);
            }
        };

        // Wait a bit to catch any async errors
        await Page.WaitForTimeoutAsync(500);
        return errors.Any();
    }

    public async Task<Dictionary<string, object>> GetPerformanceMetricsAsync()
    {
        return await Page.EvaluateAsync<Dictionary<string, object>>(@"
            () => {
                const timing = performance.timing;
                const navigation = performance.navigation;
                
                return {
                    loadTime: timing.loadEventEnd - timing.navigationStart,
                    domReady: timing.domContentLoadedEventEnd - timing.navigationStart,
                    firstByte: timing.responseStart - timing.navigationStart,
                    dnsTime: timing.domainLookupEnd - timing.domainLookupStart,
                    tcpTime: timing.connectEnd - timing.connectStart,
                    redirectCount: navigation.redirectCount,
                    type: navigation.type
                };
            }
        ");
    }

    public override async Task WaitForPageLoadAsync()
    {
        await base.WaitForPageLoadAsync();
        await WaitForContentToLoadAsync();
    }
}
