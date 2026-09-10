using Chat.Application.Abstractions;
using Chat.Application.Contracts;

using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Hubs;

public sealed class SignalRChatNotifier(IHubContext<ChatHub, IChatClient> hubContext) : IChatNotifier
{
    public Task MessagePublishedAsync(MessageDto message, CancellationToken ct) =>
        hubContext.Clients.Group($"room:{message.RoomId}").MessageReceived(message);
}