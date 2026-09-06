using Chat.Domain.Common;
using Chat.Domain.Identifiers;
using Chat.Domain.Messages;
using Chat.Domain.Messages.Events;
using Chat.Domain.Rooms;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.UnitTests.Messages;

public sealed class MessageTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 5, 12,0,0, TimeSpan.Zero);

    private static ChatRoom NewRoomOwnedBy(UserId ownerId) =>
        ChatRoom.Create(RoomId.New(), RoomName.Create("general"), ownerId, Now);

    [Fact]
    public void Send_ShouldThrowDomainException_WhenAuthorIsNotMember()
    {
        var room = NewRoomOwnedBy(UserId.New());
        var stranger = UserId.New();
        
        var act = () => Message.Send(room, stranger, MessageBody.Create("hi"), Guid.NewGuid(), Now);
        
        act.Should().Throw<DomainException>()
            .Which.Code.Should().Be("room.not_member");
    }

    [Fact]
    public void Send_ShouldRaiseMessageSentWithBodyAndTimestamp_WhenAuthorIsMember()
    {
        var owner = UserId.New();
        var room = NewRoomOwnedBy(owner);
        
        var message = Message.Send(room, owner, MessageBody.Create("hi"), Guid.NewGuid(), Now);
        
        message.SentAtUtc.Should().Be(Now);
        message.Body.Should().Be(MessageBody.Create("hi"));
        message.DomainEvents.Should().ContainSingle(e =>
            e is MessageSent);
    }

    [Fact]
    public void Send_ShouldUseProvidedClientMessageId_Always()
    {
        var owner = UserId.New();
        var room = NewRoomOwnedBy(owner);
        var clientMessageId = Guid.NewGuid();
        
        var message = Message.Send(room, owner, MessageBody.Create("hi"), clientMessageId, Now);
        
        message.ClientMessageId.Should().Be(clientMessageId);
    }
}