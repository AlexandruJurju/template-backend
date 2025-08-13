using FluentAssertions;
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
        _homePage = new HomePage(Page, BaseUrl);
    }

    [Test]
    [Description("Loading page has no errors")]
    public async Task HomePage_ShouldHaveNoErrors()
    {
        bool result = await _homePage.HasConsoleErrorsAsync();
        Assert.That(result, Is.False);
    }

    [Test]
    [Description("Verify that the page title is TemplateUi")]
    public async Task HomePage_Should_Have_Correct_Title()
    {
        await Page.GotoAsync(BaseUrl);
        await WaitForAngularAsync();
        string title = await Page.TitleAsync();

        title.Should().Be("TemplateUi", "the page title should be TemplateUi");
    }

    [Test]
    public async Task HomePage_Should_Have_Button()
    {
        await Page.GotoAsync(BaseUrl);
        await WaitForAngularAsync();

        IReadOnlyList<IElementHandle> buttons = await Page.QuerySelectorAllAsync("button");

        buttons.Should().NotBeEmpty("the page should have at least one button");

        if (buttons.Any())
        {
            string? firstButtonText = await buttons[0].TextContentAsync();
            await TestContext.Out.WriteLineAsync($"First button text: {firstButtonText}");
        }
    }

    [Test]
    [Category("Navigation")]
    [Description("Verify clicking 'Go to TEST' button navigates to test page")]
    public async Task GoToTest_Button_Should_Navigate_To_Test_Page()
    {
        await _homePage.NavigateToAsync();

        await _homePage.TakeScreenshotAsync("Before_Navigation");
        TestPage testPage = await _homePage.ClickGoToTestButtonAsync();

        bool isOnTestPage = testPage.IsOnTestPageAsync();
        bool hasExpectedText = await testPage.HasExpectedTextAsync();
        string? pageText = await testPage.GetPageTextAsync();

        isOnTestPage.Should().BeTrue("should be on /test page");
        hasExpectedText.Should().BeTrue("test page should display 'test-page works!'");
        pageText.Should().Be("test-page works!", "page should display the expected text");

        await testPage.TakeScreenshotAsync("Test_Page_After_Navigation");
        await TestContext.Out.WriteLineAsync($"Navigated to: {Page.Url}");
        await TestContext.Out.WriteLineAsync($"Page text: {pageText}");
    }
}
