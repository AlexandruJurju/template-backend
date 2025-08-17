using Microsoft.Playwright;
using Template.AcceptanceTests.Config;
using Template.AcceptanceTests.Utilities;

namespace Template.AcceptanceTests.PageObjects;

public abstract class BasePage(IPage page, string pageUrl)
{
    protected readonly IPage Page = page;
    protected readonly string PageUrl = pageUrl;
    private readonly TestSettings _settings = TestSettings.Instance;

    public async Task NavigateToAsync(string path = "")
    {
        var url = string.IsNullOrEmpty(path) ? PageUrl : $"{PageUrl}/{path.TrimStart('/')}";
        await Page.GotoAsync(url);
        await WaitForPageLoadAsync();
    }

    public virtual async Task WaitForPageLoadAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await WaitHelpers.WaitForAngularAsync(Page);
    }

    public async Task TakeScreenshotAsync(string name)
    {
        if (_settings.Screenshot)
        {
            var screenshotPath = Path.Combine(_settings.ScreenshotDir, $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            Directory.CreateDirectory(_settings.ScreenshotDir);

            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
        }
    }

    protected async Task<bool> IsElementVisibleAsync(string selector, int timeout = 5000)
    {
        return await WaitHelpers.WaitForElementAsync(Page, selector, timeout);
    }

    protected async Task<string?> GetTextContentAsync(string selector)
    {
        IElementHandle? element = await Page.QuerySelectorAsync(selector);
        return element != null ? await element.TextContentAsync() : null;
    }

    protected async Task<bool> HasClassAsync(string selector, string className)
    {
        IElementHandle? element = await Page.QuerySelectorAsync(selector);

        if (element == null)
        {
            return false;
        }

        var classes = await element.GetAttributeAsync("class");
        return classes?.Contains(className) ?? false;
    }
}
