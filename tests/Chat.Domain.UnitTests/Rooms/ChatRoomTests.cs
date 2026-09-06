using Chat.Domain.Common;
using Chat.Domain.Identifiers;
using Chat.Domain.Rooms;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.UnitTests.Rooms;

public sealed class ChatRoomTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 5, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    private void Create_ShouldAddOwnerAsMember_Always()
    {
        var ownerId = UserId.New();

        var room = ChatRoom.Create(RoomId.New(), RoomName.Create("general"), ownerId, Now);

        room.Members.Should().ContainSingle(m => m.UserId == ownerId);
    }

    [Fact]
    public void Create_ShouldRaiseRoomCreated_Always()
    {
        var room = ChatRoom.Create(RoomId.New(), RoomName.Create("general"), UserId.New(), Now);

        room.DomainEvents.Should().ContainSingle(e => e is RoomCreated);
    }

    [Fact]
    public void Join_ShouldAddMemberAndRaiseMemberJoined_Always()
    {
        var room = ChatRoom.Create(RoomId.New(), RoomName.Create("general"), UserId.New(), Now);
        var joiner = UserId.New();
        
        var joined = room.Join(joiner, Now);
        
        joined.Should().BeTrue();
    }

    [Fact]
    public void Join_ShouldReturnFalseAndRaiseNoEvent_WhenUserIsAlreadyMember()
    {
        var ownerId = UserId.New();
        var room = ChatRoom.Create(RoomId.New(), RoomName.Create("general"), ownerId, Now);
        room.ClearDomainEvents();
        
        var joined = room.Join(ownerId, Now);
        
        joined.Should().BeFalse();
        room.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void EnsureCanPost_ShouldThrowDomainException_WhenUserIsNotMember()
    {
        var room = ChatRoom.Create(RoomId.New(), RoomName.Create("general"), UserId.New(), Now);
        var stranger = UserId.New();

        var act = () => room.EnsureCanPost(stranger);

        act.Should().Throw<DomainException>()
            .Which.Code.Should().Be("room.not_member");
    }
}