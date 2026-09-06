using Chat.Domain.Identifiers;

namespace Chat.Domain.Messsages.Events;

public sealed record MessageSent(
    MessageId Id,
    RoomId RoomId,
    UserId AuthorId,
    string Body,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset OccuredAtUtc
    );