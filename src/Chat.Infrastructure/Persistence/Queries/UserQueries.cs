using Chat.Application.Abstractions;
using Chat.Domain.Identifiers;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Persistence.Queries;

internal sealed class UserQueries(ChatDbContext db) : IUserQueries
{
    public async Task<string?> GetUserNameAsync(UserId id, CancellationToken ct)
    {
        var userName = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => u.UserName)
            .FirstOrDefaultAsync(ct);

        return userName?.Value;
    }
}