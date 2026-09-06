using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Contracts;

namespace Chat.Application.Features.Rooms.CreateRoom;

public sealed class CreateRoomHandler(
        IRoomRepository rooms,
        IUserContext userContext,
        IUnitOfWork unitOfWork,
        IClock clock
    )
{
    public async Task<Result<RoomDto>> HandleAsync(CreateRoomCommand request, CancellationToken ct)
        => throw new NotImplementedException();
}