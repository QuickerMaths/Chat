using Chat.Domain.Common;
using Chat.Domain.Identifiers;

namespace Chat.Domain.Rooms;

public sealed record MemberJoined(
    RoomId RoomId,
    UserId UserId,
    DateTimeOffset OccuredAtUtc): IDomainEvent;