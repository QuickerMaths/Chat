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
    {
        var room = await rooms.FindByIdAsync(command.id, ct);

        if (room is null)
            return Result<bool>.Failure(ApplicationError.NotFound("room.not_found", "This room does not exist."));

        var joined = room.Join(userContext.UserId, clock.UtcNow);
        if (!joined)
            return Result<bool>.Success(false);

        await unitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}