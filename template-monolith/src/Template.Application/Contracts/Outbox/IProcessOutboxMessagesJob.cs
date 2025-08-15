namespace Template.Application.Contracts.Outbox;

public interface IProcessOutboxMessagesJob
{
    Task ProcessAsync();
}
