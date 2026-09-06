using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Contracts;
using Chat.Domain.Common;
using Chat.Domain.Identifiers;
using Chat.Domain.Messages;
using Chat.Domain.ValueObjects;

namespace Chat.Application.Features.Messages.SendMessage;

public sealed class SendMessageHandler(
        IRoomRepository rooms,
        IMessageRepository messages,
        IUserQueries users,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        IClock clock
    )
{
    public async Task<Result<MessageDto>> HandleAsync(SendMessageCommand command, CancellationToken ct)
    {
        var roomId = new  RoomId(command.RoomId);
        var room = await rooms.FindByIdAsync(roomId, ct);

        if (room is null)
            return Result<MessageDto>.Failure(ApplicationError.NotFound("room.not_found", "This room does not exits"));

        var existing = await messages.FindByClientIdAsync(roomId, command.ClientMessageId, ct);
        if (existing is not null)
            return Result<MessageDto>.Success(await ToDtoAsync(existing, ct));
        
        var body = MessageBody.Create(command.Body);
        Message message;

        try
        {
            message = Message.Send(room, userContext.UserId, body, command.ClientMessageId, clock.UtcNow);
        }
        catch (DomainException ex) when (ex.Code == "room.not_member")
        {
            return Result<MessageDto>.Failure(ApplicationError.Forbidden("room.not_member",
                "You are not a member of this room"));
        }

        messages.Add(message);
        await unitOfWork.SaveChangesAsync(ct);
        return Result<MessageDto>.Success(await ToDtoAsync(message, ct));
    }

    private async Task<MessageDto> ToDtoAsync(Message message, CancellationToken ct)
    {
        var authorName = await users.GetUserNameAsync(message.AuthorId, ct) ?? "unknown";
        return new MessageDto(
            message.Id.Value,
            message.RoomId.Value,
            message.AuthorId.Value,
            authorName,
            message.Body.Value,
            message.SentAtUtc
            );
    }
}