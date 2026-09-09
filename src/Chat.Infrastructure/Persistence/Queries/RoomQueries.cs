using Chat.Application.Abstractions;
using Chat.Application.Contracts;
using Chat.Domain.Identifiers;

using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Persistence.Queries;

internal sealed class RoomQueries(ChatDbContext db) : IRoomQueries
{
    public async Task<IReadOnlyList<RoomListItemDto>> FindAsync(
        UserId currentUser, CancellationToken ct)
    {
        var rows = await db.ChatRooms
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new
            {
                r.Id,
                r.Name,
                IsMember = r.Members.Any(m => m.UserId == currentUser),
                MemberCount = r.Members.Count,
                r.CreatedAtUtc
            })
            .ToListAsync(ct);

        return rows
            .Select(r => new RoomListItemDto(
                r.Id.Value, r.Name.Value, r.IsMember, r.MemberCount, r.CreatedAtUtc))
            .ToList();
    }
}