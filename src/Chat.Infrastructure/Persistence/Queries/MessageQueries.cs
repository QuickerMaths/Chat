using Chat.Application.Abstractions;
using Chat.Application.Contracts;
using Chat.Domain.Identifiers;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Persistence.Queries;

internal sealed class MessageQueries(ChatDbContext db) : IMessageQueries
{
    public async Task<IReadOnlyList<MessageDto>> FindPageAsync(
        RoomId roomId, DateTimeOffset? before, int limit, CancellationToken ct)
    {
        var query = db.Messages
            .AsNoTracking()
            .Where(m => m.RoomId == roomId);

        if (before is { } cursor)
        {
            query = query.Where(m => m.SentAtUtc < cursor);
        }

        var rows = await query
            .Join(
                db.Users,
                message => message.AuthorId,
                user => user.Id,
                (message, user) => new { message, user })
            .OrderByDescending(x => x.message.SentAtUtc)
            .ThenByDescending(x => x.message.Id)
            .Take(limit)
            .Select(x => new
            {
                x.message.Id,
                x.message.RoomId,
                x.message.AuthorId,
                AuthorName = x.user.UserName,
                x.message.Body,
                x.message.SentAtUtc
            })
            .ToListAsync(ct);

        var page = rows
            .Select(x => new MessageDto(
                x.Id.Value, x.RoomId.Value, x.AuthorId.Value,
                x.AuthorName.Value, x.Body.Value, x.SentAtUtc))
            .ToList();

        page.Reverse();
        return page;
    }
}