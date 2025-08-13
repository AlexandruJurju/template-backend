using Microsoft.Playwright;

namespace Template.AcceptanceTests.PageObjects;

public class TestPage(IPage page, string pageUrl) : BasePage(page, pageUrl)
{
    private const string TestPageText = "p:has-text('test-page works!')";
    private const string PageContent = "p";

    public bool IsOnTestPageAsync()
    {
        string url = Page.Url;
        return url.Contains("/test");
    }

    public async Task<bool> HasExpectedTextAsync()
    {
        return await IsElementVisibleAsync(TestPageText);
    }

    public async Task<string?> GetPageTextAsync()
    {
        IElementHandle? element = await Page.QuerySelectorAsync(PageContent);
        return element != null ? await element.TextContentAsync() : null;
    }

    public override async Task WaitForPageLoadAsync()
    {
        await base.WaitForPageLoadAsync();
        await Page.WaitForSelectorAsync(TestPageText, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 5000
        });
    }
}
