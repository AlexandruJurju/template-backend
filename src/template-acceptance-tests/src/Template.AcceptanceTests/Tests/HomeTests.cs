using Microsoft.Playwright;
using Template.AcceptanceTests.PageObjects;

namespace Template.AcceptanceTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
internal sealed class HomePageTests : BaseTest
{
    private HomePage _homePage = null!;

    [SetUp]
    public new async Task SetUp()
    {
        await base.SetUp();
        _homePage = new HomePage(Page, $"{BaseUrl}/home");
    }

    [Test]
    public async Task HomePage_ShouldHaveNoErrors()
    {
        var result = await _homePage.HasConsoleErrorsAsync();
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HomePage_Should_Have_Correct_Title()
    {
        await Page.GotoAsync(BaseUrl);
        await WaitForAngularAsync();
        var title = await Page.TitleAsync();

        Assert.That(title, Is.Not.Null);
    }

    [Test]
    public async Task HomePage_Should_Have_Button()
    {
        await Page.GotoAsync(BaseUrl);
        await WaitForAngularAsync();

        IReadOnlyList<IElementHandle> buttons = await Page.QuerySelectorAllAsync("button");

        Assert.That(buttons, Is.Not.Empty);

        if (buttons.Any())
        {
            var firstButtonText = await buttons[0].TextContentAsync();
            await TestContext.Out.WriteLineAsync($"First button text: {firstButtonText}");
        }
    }

    [Test]
    public async Task GoToTest_Button_Should_Navigate_To_Test_Page()
    {
        await _homePage.NavigateToAsync();

        await _homePage.TakeScreenshotAsync("Before_Navigation");
        TestPage testPage = await _homePage.ClickGoToTestButtonAsync();

        var isOnTestPage = testPage.IsOnTestPageAsync();
        var hasExpectedText = await testPage.HasExpectedTextAsync();
        var pageText = await testPage.GetPageTextAsync();

        Assert.That(pageText, Is.EqualTo("test-page works!"));
        Assert.That(isOnTestPage, Is.True);
        Assert.That(hasExpectedText, Is.True);

        await testPage.TakeScreenshotAsync("Test_Page_After_Navigation");
        await TestContext.Out.WriteLineAsync($"Navigated to: {Page.Url}");
        await TestContext.Out.WriteLineAsync($"Page text: {pageText}");
    }
}
