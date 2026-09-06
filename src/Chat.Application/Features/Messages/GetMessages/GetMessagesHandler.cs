using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Contracts;

namespace Chat.Application.Features.Messages.GetMessages;

public sealed class GetMessagesHandler(
        IRoomRepository rooms,
        IMessageQueries messages,
        IUserContext userContext
    )
{
    private const int DefaultLimit = 50;
    private const int MaxLimit = 100;

    public async Task<Result<IReadOnlyList<MessageDto>>> HandleAsync(GetMessagesCommand command, CancellationToken ct)
        => throw new NotImplementedException();
}