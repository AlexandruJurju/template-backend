namespace Template.Application.Abstractions.Outbox;

public interface IProcessOutboxMessagesJob
{
    Task ProcessAsync();
}
