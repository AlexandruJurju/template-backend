using Aspire.Hosting;

namespace Template.E2ETests;

[TestFixture]
public class MonolithTests : VerifyBase
{
    private DistributedApplication _app = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        IDistributedApplicationTestingBuilder appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Template_AspireHost>();
        _app = await appHost.BuildAsync();
        await _app.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _app.DisposeAsync();
    }

    [Test]
    public async Task GetRoot_ReturnsExpectedResponse()
    {
        // Arrange
        using HttpClient httpClient = _app.CreateHttpClient("api");

        // Act
        HttpResponseMessage response = await httpClient.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();

        var result = new
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = response.Content.Headers.ContentType?.ToString()
        };

        await Verify(result);
    }
}
