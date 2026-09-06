using Chat.Domain.Identifiers;

namespace Chat.Domain.Rooms;

public sealed class RoomMember
{
    public UserId UserId { get; init; }
    
    public DateTimeOffset JoinedAtUtc { get; init; }
}
