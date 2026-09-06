using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Contracts;
using Chat.Domain.Identifiers;
using Chat.Domain.Rooms;
using Chat.Domain.ValueObjects;

namespace Chat.Application.Features.Rooms.CreateRoom;

public sealed class CreateRoomHandler(
        IRoomRepository rooms,
        IUserContext userContext,
        IUnitOfWork unitOfWork,
        IClock clock
    )
{
    public async Task<Result<RoomDto>> HandleAsync(CreateRoomCommand command, CancellationToken ct)
    {
        var name = RoomName.Create(command.Name);

        if (await rooms.ExistsWithNameAsync(name, ct))
            return Result<RoomDto>.Failure(ApplicationError.Conflict("room.name_taken",
                "Room with this name already exists."));
        
        var room = ChatRoom.Create(RoomId.New(), name, userContext.UserId, clock.UtcNow);
        
        rooms.Add(room);
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result<RoomDto>.Success(new RoomDto(room.Id.Value, room.Name.Value, room.CreatedAtUtc));
    }
}