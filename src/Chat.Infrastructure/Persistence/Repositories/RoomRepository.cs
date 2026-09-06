using Chat.Application.Abstractions;
using Chat.Domain.Identifiers;
using Chat.Domain.Rooms;
using Chat.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Persistence.Repositories;

internal sealed class RoomRepository(ChatDbContext db) : IRoomRepository
{
    public void Add(ChatRoom room) => db.ChatRooms.Add(room);

    public Task<ChatRoom?> FindByIdAsync(RoomId id, CancellationToken ct) =>
        db.ChatRooms.FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<bool> ExistsWithNameAsync(RoomName name, CancellationToken ct) =>
        db.ChatRooms.AnyAsync(r => r.Name == name, ct);
}