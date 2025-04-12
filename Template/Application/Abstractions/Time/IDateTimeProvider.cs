namespace Application.Abstractions.Infrastructure;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}