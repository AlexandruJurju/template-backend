using Microsoft.Playwright;
using Template.AcceptanceTests.Config;
using Template.AcceptanceTests.Utilities;

namespace Template.AcceptanceTests.PageObjects;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly string BaseUrl;
    protected readonly TestSettings Settings;

#pragma warning disable CA1054
    protected BasePage(IPage page, string baseUrl)
#pragma warning restore CA1054
    {
        Page = page;
        BaseUrl = baseUrl;
        Settings = TestSettings.Instance;
    }

    public async Task<T> NavigateToAsync<T>(string path = "") where T : BasePage
    {
        string url = string.IsNullOrEmpty(path) ? BaseUrl : $"{BaseUrl}/{path.TrimStart('/')}";
        await Page.GotoAsync(url);
        await WaitForPageLoadAsync();
        return (T)Activator.CreateInstance(typeof(T), Page, BaseUrl)!;
    }

    public virtual async Task WaitForPageLoadAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await WaitHelpers.WaitForAngularAsync(Page);
    }

    public async Task TakeScreenshotAsync(string name)
    {
        if (Settings.Screenshot)
        {
            string screenshotPath = Path.Combine(Settings.ScreenshotDir, $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            Directory.CreateDirectory(Settings.ScreenshotDir);
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
        }
    }

    public async Task<bool> IsElementVisibleAsync(string selector, int timeout = 5000)
    {
        return await WaitHelpers.WaitForElementAsync(Page, selector, timeout);
    }

    public async Task ScrollToElementAsync(string selector)
    {
        await Page.EvaluateAsync($"document.querySelector('{selector}')?.scrollIntoView({{behavior: 'smooth', block: 'center'}})");
        await Page.WaitForTimeoutAsync(500); // Wait for scroll animation
    }

    public async Task<string?> GetTextContentAsync(string selector)
    {
        IElementHandle? element = await Page.QuerySelectorAsync(selector);
        return element != null ? await element.TextContentAsync() : null;
    }

    public async Task<bool> HasClassAsync(string selector, string className)
    {
        IElementHandle? element = await Page.QuerySelectorAsync(selector);
        if (element == null)
        {
            return false;
        }

        string? classes = await element.GetAttributeAsync("class");
        return classes?.Contains(className) ?? false;
    }
}
