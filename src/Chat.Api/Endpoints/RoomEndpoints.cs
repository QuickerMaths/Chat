using Chat.Application.Features.Rooms.CreateRoom;
using Chat.Application.Features.Rooms.JoinRoom;
using Chat.Application.Features.Rooms.ListRooms;
using Chat.Domain.Identifiers;

namespace Chat.Api.Endpoints;

public static class RoomEndpoints
{
    public static void MapRoom(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rooms").RequireAuthorization();
        
        // GET /api/rooms -> 200 RoomListItemDto[]
        group.MapGet("/",
            async (ListRoomsHandler handler, CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(ct);
                return result.ToHttpResult(Results.Ok);
            });
        
        // POST /api/rooms -> 201 RoomDto + Location / 409 (room.name_taken)
        group.MapPost("/",
            async (CreateRoomCommand command, CreateRoomHandler handler, CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(command, ct);
                return result.ToHttpResult(room => Results.Created($"/api/rooms/{room.Id}", room));
            })
            .AddEndpointFilter<ValidationFilter<CreateRoomCommand>>();
        
        // POST /api/rooms/{roomId}/members -> 204 (joined OR already a member) / 404 (room.not_found)
        group.MapPost("/{roomId:guid}/members",
            async (Guid roomId, JoinRoomHandler handler, CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(new JoinRoomCommand(new RoomId(roomId)), ct);
                return result.ToHttpResult(_ => Results.NoContent());
            });
    }
}