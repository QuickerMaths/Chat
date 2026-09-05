namespace Chat.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccuredAtUtc { get; }
}