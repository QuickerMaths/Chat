using Chat.Domain.Common;
using Chat.Domain.Identifiers;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Users;

public sealed class User: Entity<UserId>
{
    private User() {}

    public UserName UserName { get; private init; } = null!;

    public string PasswordHash { get; private init; } = null!;
    
    public DateTimeOffset CreatedAtUtc { get; private init; }

    public static User Register(UserId id, UserName userName, string passwordHash, DateTimeOffset nowUtc)
        => new() { Id = id, UserName = userName, PasswordHash = passwordHash, CreatedAtUtc = nowUtc };
}