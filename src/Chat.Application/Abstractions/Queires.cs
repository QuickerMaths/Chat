using Chat.Application.Contracts;
using Chat.Domain.Identifiers;

namespace Chat.Application.Abstractions;

public interface IRoomQueries
{
    Task<IReadOnlyList<RoomListItemDto>> FindAsync(UserId currentUser, CancellationToken ct );
}

public interface IMessageQueries
{
    Task<IReadOnlyList<MessageDto>> FindPageAsync(RoomId roomId, DateTimeOffset? before, int limit, CancellationToken ct );
}

public interface IUserQueries
{
    Task<string?> GetUserNameAsync(UserId userId, CancellationToken ct );
}