using Chat.Application.Abstractions;
using Chat.Domain.Identifiers;
using Chat.Domain.Messages;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Persistence.Repositories;

internal sealed class MessageRepository(ChatDbContext db) : IMessageRepository
{
    public void Add(Message message) => db.Messages.Add(message);

    public Task<Message?> FindByClientIdAsync(
        RoomId roomId, Guid clientMessageId, CancellationToken ct) =>
        db.Messages.FirstOrDefaultAsync(
            m => m.RoomId == roomId && m.ClientMessageId == clientMessageId, ct);
}