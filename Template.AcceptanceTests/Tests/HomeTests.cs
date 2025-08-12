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
}
