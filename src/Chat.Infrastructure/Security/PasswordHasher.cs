using Chat.Application.Abstractions;
using Chat.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Chat.Infrastructure.Security;

internal sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _inner = new();

    public string Hash(string password) => _inner.HashPassword(user: null!, password);

    public bool Verify(string passwordHash, string password)
    {
        var result = _inner.VerifyHashedPassword(user: null!, passwordHash, password);
        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}