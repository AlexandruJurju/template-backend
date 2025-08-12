using Microsoft.Playwright;
using Template.AcceptanceTests.Utilities;

namespace Template.AcceptanceTests.PageObjects;

public class HomePage : BasePage
{
    // Selectors - Update these based on your Angular app's actual selectors
    private const string AppRoot = "app-root";
    private const string NavigationMenu = "nav, .navbar, header nav";
    private const string MainContent = "main, .main-content, router-outlet";
    private const string Title = "h1, .page-title, .hero-title";
    private const string LoadingSpinner = ".spinner, .loading, .loader";

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

    public async Task<string?> GetPageTitleAsync()
    {
        return await GetTextContentAsync(Title);
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

    public async Task<List<string>> GetNavigationLinksAsync()
    {
        IReadOnlyList<IElementHandle> links = await Page.QuerySelectorAllAsync($"{NavigationMenu} a");
        var linkTexts = new List<string>();

        foreach (IElementHandle link in links)
        {
            string? text = await link.TextContentAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                linkTexts.Add(text.Trim());
            }
        }

        return linkTexts;
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
