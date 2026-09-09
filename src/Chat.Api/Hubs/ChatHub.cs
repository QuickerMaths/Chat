using Chat.Application.Contracts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Hubs;

public interface IChatClient
{
    Task MessageReceived(MessageDto message);
}

[Authorize]
public sealed class ChatHub: Hub<IChatClient>
{
    public Task JoinRoom(Guid roomId) =>
    Groups.AddToGroupAsync(Context.ConnectionId, $"room:{roomId}");
    
    public Task LeaveRoom(Guid roomId) =>
    Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room:{roomId}");
}