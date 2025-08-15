namespace Template.Application.Contracts.Services;

public interface IRandomNumberService
{
    List<int> GenerateRandomNumbers(int count);
    Task BroadcastRandomNumbers(List<int> numbers);
    Task BroadcastToGroup(string groupName, List<int> numbers);
}
