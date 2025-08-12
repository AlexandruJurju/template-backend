using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Template.AcceptanceTests.Config;

namespace Template.AcceptanceTests.Fixtures;

public class WebApplicationFixture
{
    private DistributedApplication? _app;
    public string BaseUrl { get; private set; } = string.Empty;
    public string ApiUrl { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        if (!TestSettings.Instance.UseAspireHost)
        {
            BaseUrl = TestSettings.Instance.BaseUrl;
            ApiUrl = TestSettings.Instance.ApiBaseUrl;
            return;
        }

        IDistributedApplicationTestingBuilder appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Template_AspireHost>();

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            clientBuilder.ConfigureHttpClient(client => client.Timeout = TimeSpan.FromMinutes(5)));

        _app = await appHost.BuildAsync();
        await _app.StartAsync();

        // Get the URLs from the running application
        ResourceNotificationService resourceNotificationService = _app.Services
            .GetRequiredService<ResourceNotificationService>();

        await resourceNotificationService.WaitForResourceAsync("template-ui")
            .WaitAsync(TimeSpan.FromSeconds(60));

        await resourceNotificationService.WaitForResourceAsync("template-api")
            .WaitAsync(TimeSpan.FromSeconds(60));

        BaseUrl = TestSettings.Instance.BaseUrl;

        ApiUrl = TestSettings.Instance.ApiBaseUrl;
    }

    public async Task DisposeAsync()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
        }
    }
}
