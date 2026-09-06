using Chat.Domain.Common;
using Chat.Domain.Identifiers;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Rooms;

public sealed record RoomCreated(
    RoomId RoomId,
    RoomName Name,
    UserId OwnerId,
    DateTimeOffset OccuredAtUtc) : IDomainEvent;