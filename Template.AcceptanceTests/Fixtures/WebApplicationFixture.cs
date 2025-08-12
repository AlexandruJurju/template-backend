using System.Diagnostics;
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

        ResourceNotificationService resourceNotificationService = _app.Services
            .GetRequiredService<ResourceNotificationService>();

        await resourceNotificationService.WaitForResourceAsync("template-ui", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(60));

        await resourceNotificationService.WaitForResourceAsync("template-api", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(60));

        BaseUrl = TestSettings.Instance.BaseUrl;
        ApiUrl = TestSettings.Instance.ApiBaseUrl;

        // Wait until http call to ui project returns a response that isn't 5XX
        await WaitForServiceToBeReady(BaseUrl, TimeSpan.FromMinutes(2));
        // await WaitForServiceToBeReady(ApiUrl, TimeSpan.FromMinutes(2));
    }

    private async Task WaitForServiceToBeReady(string url, TimeSpan timeout)
    {
        using var httpClient = new HttpClient();
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                Console.WriteLine($"Service at {url} is ready (Status: {response.StatusCode})");
                return;
            }
            catch (HttpRequestException)
            {
                // Service not ready yet, continue waiting
                await Task.Delay(500);
            }
            catch (TaskCanceledException)
            {
                // Timeout on individual request, continue waiting
                await Task.Delay(500);
            }
        }

        throw new TimeoutException($"Service at {url} did not become ready within {timeout}");
    }

    public async Task DisposeAsync()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
        }
    }
}
