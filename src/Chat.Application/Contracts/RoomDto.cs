namespace Chat.Application.Contracts;

public sealed record RoomDto(
        Guid Id,
        string Name,
        DateTimeOffset CreatedAtUtc
    );