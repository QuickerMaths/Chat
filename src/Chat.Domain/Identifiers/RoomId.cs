namespace Chat.Domain.Identifiers;

public readonly record struct RoomId(Guid Value)
{
    public static RoomId New() => new(Guid.CreateVersion7());
}