using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Contracts;
using Chat.Domain.Identifiers;

namespace Chat.Application.Features.Messages.GetMessages;

public sealed class GetMessagesHandler(
        IRoomRepository rooms,
        IMessageQueries messages,
        IUserContext userContext
    )
{
    private const int DefaultLimit = 50;
    private const int MaxLimit = 100;

    public async Task<Result<IReadOnlyList<MessageDto>>> HandleAsync(GetMessagesCommand command, CancellationToken ct)
    {
        var roomId = new RoomId(command.RoomId);
        
        var room = await rooms.FindByIdAsync(roomId, ct);
        if (room is null)
            return Result<IReadOnlyList<MessageDto>>.Failure(ApplicationError.NotFound("room.not_found", "This room does not exist."));

        if (!room.Members.Any(m => m.UserId == userContext.UserId))
            return Result<IReadOnlyList<MessageDto>>.Failure(ApplicationError.Forbidden("room.not_member",
                "You are not a member of this room"));

        var limit = command.Limit ?? DefaultLimit;
        if (limit > MaxLimit)
            limit = MaxLimit;

        var page = await messages.FindPageAsync(roomId, command.Before, limit, ct);
        return Result<IReadOnlyList<MessageDto>>.Success(page);
    }
}