using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Contracts;

namespace Chat.Application.Features.Rooms.ListRooms;

public sealed class ListRoomsHandler(
    IRoomQueries rooms,
    IUserContext user
    )
{
    public async Task<Result<IReadOnlyList<RoomListItemDto>>> HandleAsync(CancellationToken ct)
    {
        var items = await rooms.FindAsync(user.UserId, ct);
        return Result<IReadOnlyList<RoomListItemDto>>.Success(items);
    }
}