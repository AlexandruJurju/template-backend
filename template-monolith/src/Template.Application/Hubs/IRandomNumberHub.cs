namespace Template.Application.Hubs;

public interface IRandomNumberHub
{
    Task ReceiveRandomNumbers(List<int> numbers);
    Task Connected(string connectionId);
    Task UserJoined(string connectionId, string groupName);
    Task UserLeft(string connectionId, string groupName);
}
