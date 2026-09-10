using Chat.Domain.Common;
using Chat.Domain.Identifiers;
using Chat.Domain.Messages.Events;
using Chat.Domain.Rooms;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Messages;

public sealed class Message : Entity<MessageId>
{
    private Message()
    {}
    
    public RoomId RoomId {get; private init;}
    
    public UserId AuthorId {get; private init;}

    public MessageBody Body { get; private init; } = null!;
    
    public DateTimeOffset SentAtUtc {get; private init;}
    
    public Guid ClientMessageId {get; private init;}

    public static Message Send(
        ChatRoom room,
        UserId authorId,
        MessageBody body,
        Guid clientMessageId,
        DateTimeOffset nowUtc)
    {
        room.EnsureCanPost(authorId);

        var message = new Message
        {
            Id = MessageId.New(),
            RoomId = room.Id,
            AuthorId = authorId,
            Body = body,
            SentAtUtc = nowUtc,
            ClientMessageId = clientMessageId
        };
        
        message.Raise(new MessageSent(
            message.Id,
            room.Id,
            authorId,
            body.Value,
            nowUtc,
            nowUtc));
        
        return message;
    }
}