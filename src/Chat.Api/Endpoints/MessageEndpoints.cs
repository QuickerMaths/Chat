using Chat.Application.Features.Messages.GetMessages;
using Chat.Application.Features.Messages.SendMessage;

namespace Chat.Api.Endpoints;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rooms/{roomId:guid}/messages").RequireAuthorization();
        
        // GET /api/rooms/{roomId}/messages/?before=&limit= -> 200 MessageDto[] / 403 (room.not_member) / 404 (room.not_found)
        group.MapGet("/",
            async (Guid roomId, DateTimeOffset? before, int? limit, GetMessagesHandler handler, CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(new GetMessagesCommand(roomId, before, limit), ct);
                return result.ToHttpResult(Results.Ok);
            });
        
        // POST /api/rooms/{roomId}/messages -> 201 MessageDto / 403 (room.not_member) / 404 (room.not_found)
        group.MapPost("/",
            async (Guid roomId, SendMessageRequest request, SendMessageHandler handler, CancellationToken ct) =>
            {
                var command = new SendMessageCommand(roomId, request.Body, request.ClientMessageId);
                var result = await handler.HandleAsync(command, ct);
                return result.ToHttpResult(message =>
                    Results.Json(message, statusCode: StatusCodes.Status201Created));
            });
    }
}