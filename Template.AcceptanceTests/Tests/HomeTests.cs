using FluentAssertions;
using Microsoft.Playwright;
using Template.AcceptanceTests.PageObjects;

namespace Template.AcceptanceTests.Tests;

[TestFixture]
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
    [Description("Verify that the page title is TemplateUi")]
    public async Task HomePage_Should_Have_Correct_Title()
    {
        // Arrange & Act
        await Page.GotoAsync(BaseUrl);
        await WaitForAngularAsync();
        string title = await Page.TitleAsync();

        // Assert
        title.Should().Be("TemplateUi", "the page title should be TemplateUi");
    }

    [Test]
    public async Task HomePage_Should_Have_Button()
    {
        // Arrange & Act
        await Page.GotoAsync(BaseUrl);
        await WaitForAngularAsync();

        // Find all buttons on the page
        IReadOnlyList<IElementHandle> buttons = await Page.QuerySelectorAllAsync("button");

        // Assert
        buttons.Should().NotBeEmpty("the page should have at least one button");

        // Get text from first button if it exists
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
        // Arrange
        await _homePage.NavigateToAsync<HomePage>();

        // Act
        await _homePage.TakeScreenshotAsync("Before_Navigation");
        TestPage testPage = await _homePage.ClickGoToTestButtonAsync();

        // Assert
        bool isOnTestPage = testPage.IsOnTestPageAsync();
        bool hasExpectedText = await testPage.HasExpectedTextAsync();
        string? pageText = await testPage.GetPageTextAsync();

        isOnTestPage.Should().BeTrue("should be on /test page");
        hasExpectedText.Should().BeTrue("test page should display 'test-page works!'");
        pageText.Should().Be("test-page works!", "page should display the expected text");

        // Take screenshot of test page
        await testPage.TakeScreenshotAsync("Test_Page_After_Navigation");
        await TestContext.Out.WriteLineAsync($"Navigated to: {Page.Url}");
        await TestContext.Out.WriteLineAsync($"Page text: {pageText}");
    }
}
