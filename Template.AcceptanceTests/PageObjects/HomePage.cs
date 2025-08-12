using Microsoft.Playwright;
using Template.AcceptanceTests.Utilities;

namespace Template.AcceptanceTests.PageObjects;

public class HomePage(IPage page, string baseUrl) : BasePage(page, baseUrl)
{
    private const string AppRoot = "app-root";
    private const string GoToTestButton = "button:has-text('Go to TEST'), button:text('Go to TEST')";
    private const string AllButtons = "button";

    public async Task<bool> IsPageLoadedAsync()
    {
        await WaitHelpers.WaitForAngularAsync(Page);
        return await IsElementVisibleAsync(AppRoot);
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

        await Page.WaitForTimeoutAsync(500);
        return errors.Any();
    }
}
