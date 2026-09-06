using Chat.Domain.Identifiers;

namespace Chat.Application.Features.Messages.GetMessages;

public sealed record GetMessagesCommand(Guid RoomId, DateTimeOffset? Before, int? Limit);