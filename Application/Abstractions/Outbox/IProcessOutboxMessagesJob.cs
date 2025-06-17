namespace Application.Abstractions.Outbox;

public interface IProcessOutboxMessagesJob
{
    Task ProcessAsync();
}
