using Chat.Application.Contracts;
using Chat.Domain.Common;
using Chat.Domain.Users;

namespace Chat.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string passwordHash, string password);
}

public interface ITokenIssuer
{
    AccessTokenDto Issue(User user);
}

public interface IChatNotifier
{
    Task MessagePublishedAsync(MessageDto message, CancellationToken ct);
}

public interface IDomainEventsHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken ct);
}