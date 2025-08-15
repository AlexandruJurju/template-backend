using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Template.Application.Contracts.Services;
using Template.Application.Hubs;

namespace Template.Application.Services;

public class RandomNumberService(
    IHubContext<RandomNumberHub, IRandomNumberHub> hubContext,
    ILogger<RandomNumberService> logger)
    : IRandomNumberService
{
    private readonly Random _random = new();

    public List<int> GenerateRandomNumbers(int count)
    {
        return Enumerable.Range(0, count)
#pragma warning disable CA5394
            .Select(_ => _random.Next(1, 1000))
#pragma warning restore CA5394
            .ToList();
    }

    public async Task BroadcastRandomNumbers(List<int> numbers)
    {
        await hubContext.Clients.All.ReceiveRandomNumbers(numbers);
        logger.LogInformation("Broadcasted numbers: [{Join}]", string.Join(", ", numbers));
    }

    public async Task BroadcastToGroup(string groupName, List<int> numbers)
    {
        await hubContext.Clients.Group(groupName).ReceiveRandomNumbers(numbers);
        logger.LogInformation("Sent numbers to group {GroupName}: [{Join}]", groupName, string.Join(", ", numbers));
    }
}
