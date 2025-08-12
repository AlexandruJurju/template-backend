using Template.E2E.Tests;
using Xunit.Abstractions;

namespace Template.AcceptanceTests;

public class EnhancedHomePageTests : BaseTest
{
    public EnhancedHomePageTests(AspireTestFixture aspireFixture, ITestOutputHelper output)
        : base(aspireFixture, output)
    {
    }

    [Fact]
    public async Task HomePage_ShouldLoad_WithCleanState()
    {
        // Arrange - Ensure clean state (automatically done in BaseTest.InitializeAsync)
        Output.WriteLine("Starting with clean state...");

        // Act
        await NavigateToAppAsync();
        await WaitForAngularReadyAsync();

        // Assert

        // Verify UI is responsive using additional health check
        bool isResponsive = await IsUIResponsiveAsync();
        Assert.True(isResponsive, "UI should be responsive");

        Output.WriteLine("HomePage loaded successfully with responsive UI");
    }
}
