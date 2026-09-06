using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Contracts;

namespace Chat.Application.Features.Messages.SendMessage;

public sealed class SendMessageHandler(
        IRoomRepository rooms,
        IMessageRepository messages,
        IUserQueries users,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        IClock clock
    )
{
    public async Task<Result<MessageDto>> HandleAsync(SendMessageCommand command, CancellationToken ct)
        => throw new NotImplementedException();
}