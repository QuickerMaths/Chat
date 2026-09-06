using Chat.Domain.Identifiers;

namespace Chat.Application.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}

public interface IUserContext
{
    UserId UserId { get; }
    bool IsAuthenticated { get; }
}