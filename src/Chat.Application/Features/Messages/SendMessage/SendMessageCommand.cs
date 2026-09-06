namespace Chat.Application.Features.Messages.SendMessage;

public sealed record SendMessageCommand(Guid RoomId, string Body, Guid ClientMessageId);