namespace Chat.Domain.Identifiers;

public readonly record struct MessageId(Guid Value)
{
    public static MessageId New() => new(Guid.CreateVersion7());
}