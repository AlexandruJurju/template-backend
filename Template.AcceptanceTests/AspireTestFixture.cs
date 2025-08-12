using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Template.E2E.Tests;

public class AspireTestFixture : IAsyncLifetime
{
    private static DistributedApplication? _sharedApp;
    private static IDistributedApplicationTestingBuilder? _sharedAppHost;
    private static readonly SemaphoreSlim _initSemaphore = new(1, 1);
    private static bool _isInitialized;

    public string? FrontendUrl { get; private set; }
    public string? BackendUrl { get; private set; }
    public HttpClient? BackendHttpClient { get; private set; }
    public HttpClient? FrontendHttpClient { get; private set; }

    public async Task InitializeAsync()
    {
        await _initSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!_isInitialized)
            {
                await CreateSharedApplicationAsync().ConfigureAwait(false);
                _isInitialized = true;
            }

            // Create new HTTP clients for this test instance
            SetupHttpClientsAsync();
        }
        finally
        {
            _initSemaphore.Release();
        }
    }

    private static async Task CreateSharedApplicationAsync()
    {
        // Create the distributed application testing builder
        _sharedAppHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Template_AspireHost>()
            .ConfigureAwait(false);

        // Configure HTTP client defaults for resilience
        _sharedAppHost.Services.ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddStandardResilienceHandler());

        // Build and start the application
        _sharedApp = await _sharedAppHost.BuildAsync().ConfigureAwait(false);
        await _sharedApp.StartAsync().ConfigureAwait(false);

        // Wait for initial services to be ready
        await WaitForServicesHealthyAsync().ConfigureAwait(false);
    }

    private void SetupHttpClientsAsync()
    {
        if (_sharedApp is null)
        {
            throw new InvalidOperationException("Shared application not initialized");
        }

        // Dispose existing clients if any
        BackendHttpClient?.Dispose();
        FrontendHttpClient?.Dispose();

        // Create HTTP clients for both services
        BackendHttpClient = _sharedApp.CreateHttpClient("template-api");
        FrontendHttpClient = _sharedApp.CreateHttpClient("template-ui");

        // Get the actual URLs (for logging/debugging)
        BackendUrl = BackendHttpClient.BaseAddress?.ToString();
        FrontendUrl = FrontendHttpClient.BaseAddress?.ToString();
    }

    private static async Task WaitForServicesHealthyAsync()
    {
        if (_sharedApp is null)
        {
            throw new InvalidOperationException("Shared application not initialized");
        }

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        // Wait for backend API to be healthy
        await _sharedApp.ResourceNotifications.WaitForResourceHealthyAsync("template-api", cts.Token)
            .ConfigureAwait(false);

        // Wait for frontend to be healthy
        await _sharedApp.ResourceNotifications.WaitForResourceHealthyAsync("template-ui", cts.Token)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Resets the application state by recreating HTTP clients and ensuring services are healthy.
    /// Use this between tests to ensure clean state.
    /// </summary>
    public async Task ResetStateAsync()
    {
        if (_sharedApp is null)
        {
            throw new InvalidOperationException("Application not initialized");
        }

        // Dispose current HTTP clients
        BackendHttpClient?.Dispose();
        FrontendHttpClient?.Dispose();

        // Wait for services to be healthy before recreating clients
        await WaitForServicesHealthyAsync().ConfigureAwait(false);

        // Recreate HTTP clients
        SetupHttpClientsAsync();

        // Optional: Clear any caches or reset specific services
        await ResetServiceStatesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Additional UI health check method for testing purposes.
    /// This method is not used by the main testing flow but available for specific test scenarios.
    /// </summary>
    public async Task<bool> CheckUIHealthAsync(TimeSpan? timeout = null)
    {
        if (FrontendHttpClient is null)
        {
            return false;
        }

        TimeSpan timeoutSpan = timeout ?? TimeSpan.FromSeconds(10);
        using var cts = new CancellationTokenSource(timeoutSpan);

        try
        {
            // Try to hit the root of the UI application
            HttpResponseMessage response = await FrontendHttpClient.GetAsync("/", cts.Token).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            // Check if the response contains expected UI elements
            string content = await response.Content.ReadAsStringAsync(cts.Token).ConfigureAwait(false);

            // Look for Angular or typical SPA indicators
            return content.Contains("app-root") ||
                   content.Contains("ng-version") ||
                   content.Contains("angular") ||
                   content.Contains("<!DOCTYPE html>"); // At minimum, should be HTML
        }
        catch (TaskCanceledException)
        {
            return false; // Timeout
        }
        catch (HttpRequestException)
        {
            return false; // Network error
        }
        catch (Exception)
        {
            return false; // Any other error
        }
    }

    /// <summary>
    /// Performs additional service state resets like clearing caches, resetting databases to known state, etc.
    /// Override or extend this method for application-specific reset logic.
    /// </summary>
    protected virtual async Task ResetServiceStatesAsync()
    {
        if (BackendHttpClient is null)
        {
            return;
        }

        try
        {
            // Example: Clear Redis cache if your app has a cache clear endpoint
            // await BackendHttpClient.DeleteAsync("/api/cache/clear");

            // Example: Reset database to known state if you have a reset endpoint
            // await BackendHttpClient.PostAsync("/api/test/reset", null);

            // Example: Wait a bit for async operations to complete
            await Task.Delay(100).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Log but don't fail - reset is best effort
            // Consider logging here if you have a logger available
        }
    }

    /// <summary>
    /// Gets the current health status of all services
    /// </summary>
    public async Task<Dictionary<string, bool>> GetServicesHealthAsync()
    {
        var healthStatus = new Dictionary<string, bool>();

        if (_sharedApp is null)
        {
            return healthStatus;
        }

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            // Check backend health
            try
            {
                await _sharedApp.ResourceNotifications.WaitForResourceHealthyAsync("template-api", cts.Token);
                healthStatus["template-api"] = true;
            }
            catch
            {
                healthStatus["template-api"] = false;
            }

            // Check frontend health
            try
            {
                await _sharedApp.ResourceNotifications.WaitForResourceHealthyAsync("template-ui", cts.Token);
                healthStatus["template-ui"] = true;
            }
            catch
            {
                healthStatus["template-ui"] = false;
            }

            // Check UI health using our custom method
            healthStatus["ui-responsive"] = await CheckUIHealthAsync(TimeSpan.FromSeconds(3));
        }
        catch (Exception)
        {
            // Return current status even if some checks failed
        }

        return healthStatus;
    }

    public Task DisposeAsync()
    {
        // Only dispose instance-specific resources
        BackendHttpClient?.Dispose();
        FrontendHttpClient?.Dispose();

        // Note: We don't dispose _sharedApp here as it's shared across all test instances
        // The shared app will be disposed when the test process ends
        return Task.CompletedTask;
    }

    /// <summary>
    /// Static method to dispose the shared application when all tests are complete.
    /// Call this in a test assembly cleanup method if needed.
    /// </summary>
    public static async Task DisposeSharedResourcesAsync()
    {
        await _initSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_sharedApp is not null)
            {
                await _sharedApp.DisposeAsync().ConfigureAwait(false);
                _sharedApp = null;
            }

            _isInitialized = false;
        }
        finally
        {
            _initSemaphore.Release();
        }
    }
}
