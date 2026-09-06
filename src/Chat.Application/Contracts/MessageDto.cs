namespace Chat.Application.Contracts;

public record MessageDto(
    Guid Id,
    Guid RoomId,
    Guid AuthorId,
    string AuthorName,
    string Body,
    DateTimeOffset SentAtUtc
    );