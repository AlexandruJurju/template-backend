namespace Template.AcceptanceTests;

public class WeatherHubTests : BasePlaywrightTests
{
    public WeatherHubTests(AspireManager aspireManager) : base(aspireManager) { }

    [Fact]
    public async Task TestWebAppHomePage()
    {
        await ConfigureAsync<Projects.Template_AspireHost>();

        await InteractWithPageAsync("template-ui", async page =>
        {
            await page.GotoAsync("/");

            string title = await page.TitleAsync();
            Assert.Equal("TemplateUi", title);
        });
    }
}
