using Chat.Application.Abstractions;
using Chat.Application.Common;

namespace Chat.Application.Features.Rooms.JoinRoom;

public sealed class JoinRoomHandler(
        IRoomRepository rooms,
        IUserContext userContext,
        IUnitOfWork unitOfWork,
        IClock clock
    )
{
    public async Task<Result<bool>> HandleAsync(JoinRoomCommand command, CancellationToken ct)
        => throw new NotImplementedException();
}