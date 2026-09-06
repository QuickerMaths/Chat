using Chat.Domain.Common;
using Chat.Domain.Identifiers;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Rooms;

public sealed class ChatRoom : Entity<RoomId>
{
    private readonly List<RoomMember> _members = [];
    private ChatRoom()
    {
    }

    public RoomName Name { get; private set; } = null!;

    public UserId OwnerId { get; private set; }
    
    public DateTimeOffset CreatedAtUtc { get; private init; }

    public IReadOnlyCollection<RoomMember> Members => _members;

    public static ChatRoom Create(RoomId id, RoomName name, UserId ownerId, DateTimeOffset now)
    {
        var room = new ChatRoom { Id = id, Name = name, OwnerId = ownerId, CreatedAtUtc = now };
        
        room._members.Add(new RoomMember{ UserId = ownerId, JoinedAtUtc = now });
        room.Raise(new RoomCreated(id, name, ownerId, now));

        return room;
    }

    public bool Join(UserId userId, DateTimeOffset now)
    {
        if (_members.Any(m => m.UserId == userId))
        {
            return false;
        }
        
        _members.Add(new RoomMember{ UserId = userId, JoinedAtUtc = now });
        Raise(new MemberJoined(Id, userId, now));
        
        return true;
    }

    public void EnsureCanPost(UserId userId)
    {
        if (!_members.Any(m => m.UserId == userId))
        {
            throw new DomainException("room.not_member", "You must join the room before posting");
        }
    }
}
