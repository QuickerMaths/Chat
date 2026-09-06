namespace Chat.Application.Contracts;

public sealed record RoomListItemDto(
    Guid Id,
    string Name,
    bool IsMember,
    int MemberCount,
    DateTime CreatedUtcAt
    );