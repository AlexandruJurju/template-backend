using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Template.E2E.Tests;

public class AspireTestFixture : IAsyncLifetime
{
    private DistributedApplication? _app;
    private IDistributedApplicationTestingBuilder? _appHost;
    public string? FrontendUrl { get; private set; }
    public string? BackendUrl { get; private set; }
    public HttpClient? BackendHttpClient { get; private set; }
    public HttpClient? FrontendHttpClient { get; private set; }

    public async Task InitializeAsync()
    {
        // Create the distributed application testing builder
        _appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Template_AspireHost>()
            .ConfigureAwait(false);

        // Configure HTTP client defaults for resilience
        _appHost.Services.ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddStandardResilienceHandler());

        // Build and start the application
        _app = await _appHost.BuildAsync().ConfigureAwait(false);
        await _app.StartAsync().ConfigureAwait(false);

        // Wait for services to be ready and get HTTP clients
        await WaitForServicesAsync().ConfigureAwait(false);

        // Create HTTP clients for both services
        BackendHttpClient = _app.CreateHttpClient("template-api");
        FrontendHttpClient = _app.CreateHttpClient("template-ui");

        // Get the actual URLs (optional - for logging/debugging)
        BackendUrl = BackendHttpClient.BaseAddress?.ToString();
        FrontendUrl = FrontendHttpClient.BaseAddress?.ToString();
    }

    private async Task WaitForServicesAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        // Wait for backend API to be healthy
        await _app!.ResourceNotifications.WaitForResourceHealthyAsync("template-api", cts.Token)
            .ConfigureAwait(false);

        // Wait for frontend to be healthy
        await _app.ResourceNotifications.WaitForResourceHealthyAsync("template-ui", cts.Token)
            .ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        BackendHttpClient?.Dispose();
        FrontendHttpClient?.Dispose();

        if (_app is not null)
        {
            await _app.DisposeAsync().ConfigureAwait(false);
        }
    }
}
