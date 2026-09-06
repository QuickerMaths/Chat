using Chat.Domain.Identifiers;
using Chat.Domain.Messages;
using Chat.Domain.Rooms;
using Chat.Domain.Users;
using Chat.Domain.ValueObjects;

namespace Chat.Application.Abstractions;


public interface IUserRepository
{
    void Add(User user);
    Task<User?> FindByUsernameAsync(UserName userName, CancellationToken ct);
    Task<bool> ExistsAsync(UserId userId, CancellationToken ct);
}

public interface IRoomRepository
{
    void Add(ChatRoom room);
    Task<ChatRoom?> FindByIdAsync(RoomId id, CancellationToken ct);
    Task<bool> ExistsWithNameAsync(RoomName name, CancellationToken ct);
}

public interface IMessageRepository
{
    void Add(Message message);
    Task<Message?> FindByClientIdAsync(RoomId roomId, Guid clientMessageId, CancellationToken ct);
}