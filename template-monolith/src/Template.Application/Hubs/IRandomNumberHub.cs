using SignalRSwaggerGen.Attributes;

namespace Template.Application.Hubs;

[SignalRHub]
public interface IRandomNumberHub
{
    Task ReceiveRandomNumbers(List<int> numbers);
    Task Connected(string connectionId);
    Task UserJoined(string connectionId, string groupName);
    Task UserLeft(string connectionId, string groupName);
}
