using Template.E2E.Tests;
using Xunit.Abstractions;

namespace Template.AcceptanceTests;

public class HomePageTests : BaseTest
{
    public HomePageTests(AspireTestFixture aspireFixture, ITestOutputHelper output)
        : base(aspireFixture, output)
    {
    }

    [Fact]
    public async Task HomePage_ShouldLoad_Successfully()
    {
        // Arrange & Act
        await NavigateToAppAsync();

        string title = await Page!.TitleAsync();

        Assert.Equal("TemplateUi", title);
    }
}
