using Chat.Domain.Common;
using Chat.Domain.Identifiers;

namespace Chat.Domain.Messages.Events;

public sealed record MessageSent(
    MessageId MessageId,
    RoomId RoomId,
    UserId AuthorId,
    string Body,
    DateTimeOffset SentAtUtc,
    DateTimeOffset OccuredAtUtc) : IDomainEvent;