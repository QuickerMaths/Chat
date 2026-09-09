using Chat.Application.Abstractions;
using Chat.Domain.Users;
using Chat.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(ChatDbContext db): IUserRepository
{
    public void Add(User user) => db.Users.Add(user);

    public Task<User?> FindByUsernameAsync(UserName userName, CancellationToken ct)
    => db.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);

    public Task<bool> ExistsAsync(UserName userName, CancellationToken ct)
    => db.Users.AnyAsync(u => u.UserName == userName, ct);
}