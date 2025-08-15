using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Template.Application.Contracts.Services;
using Template.Application.Services;

namespace Template.Application.BackgroundServices;

public class RandomNumberBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RandomNumberBackgroundService> _logger;

    public RandomNumberBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RandomNumberBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RandomNumberBackgroundService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                IRandomNumberService randomNumberService = scope.ServiceProvider.GetRequiredService<IRandomNumberService>();

                List<int> numbers = randomNumberService.GenerateRandomNumbers(10);
                await randomNumberService.BroadcastRandomNumbers(numbers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating random numbers");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
