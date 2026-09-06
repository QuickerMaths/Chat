using Chat.Application.Abstractions;
using Chat.Application.Contracts;
using Chat.Domain.Messages.Events;

namespace Chat.Application.Features.Messages.SendMessage;

public sealed class MessageSentNotificationHandler(
    IUserQueries users,
    IChatNotifier notifier
    ) : IDomainEventsHandler<MessageSent>
{
    public async Task HandleAsync(MessageSent domainEvent, CancellationToken ct)
    {
        var authorName = await users.GetUserNameAsync(domainEvent.AuthorId, ct) ?? "unknown";

        var dto = new MessageDto(
            domainEvent.MessageId.Value,
            domainEvent.RoomId.Value,
            domainEvent.AuthorId.Value,
            authorName,
            domainEvent.Body,
            domainEvent.SentAtUtc
            );
        
        await notifier.MessagePublishedAsync(dto, ct);
    }
}