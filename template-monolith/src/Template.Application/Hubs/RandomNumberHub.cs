using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Template.Application.Services;

namespace Template.Application.Hubs;

public class RandomNumberHub(
    IRandomNumberService randomNumberService,
    ILogger<RandomNumberHub> logger)
    : Hub<IRandomNumberHub>
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).UserJoined(Context.ConnectionId, groupName);
        logger.LogInformation("User {ContextConnectionId} joined group {GroupName}", Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).UserLeft(Context.ConnectionId, groupName);
        logger.LogInformation("User {ContextConnectionId} left group {GroupName}", Context.ConnectionId, groupName);
    }

    public async Task RequestRandomNumbers()
    {
        List<int> numbers = randomNumberService.GenerateRandomNumbers(10);
        await Clients.Caller.ReceiveRandomNumbers(numbers);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.Connected(Context.ConnectionId);
        logger.LogInformation("Client connected: {ContextConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("Client disconnected: {ContextConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
