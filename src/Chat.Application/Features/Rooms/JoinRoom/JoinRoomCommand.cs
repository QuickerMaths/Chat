using Chat.Domain.Identifiers;

namespace Chat.Application.Features.Rooms.JoinRoom;

public sealed record JoinRoomCommand(RoomId id);