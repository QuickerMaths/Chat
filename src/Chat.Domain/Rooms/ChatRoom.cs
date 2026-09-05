using Chat.Domain.Common;

namespace Chat.Domain.Rooms;

public sealed class ChatRoom : Entity<Guid>
{
    public ChatRoom(Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    public string Name { get; private set; }
}
