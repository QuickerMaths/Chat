using Chat.Application.Abstractions;
using Chat.Application.Features.Rooms.CreateRoom;
using Chat.Domain.Identifiers;
using Chat.Domain.Rooms;
using Chat.Domain.ValueObjects;

namespace Chat.Application.UnitTests.Features.Rooms;

public sealed class CreateRoomsHandlerTests
{
    private readonly IRoomRepository _rooms = Substitute.For<IRoomRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly FakeClock _clock = new();
    
    public CreateRoomsHandlerTests() => _userContext.UserId.Returns(UserId.New());
    
    private CreateRoomHandler CreateHandler() => new(_rooms, _userContext, _unitOfWork, _clock);

    [Fact]
    public async Task HandleAsync_ShouldAddRoomAndSaveOnce_WhenNameIsFree()
    {
        _rooms.ExistsWithNameAsync(Arg.Any<RoomName>(), Arg.Any<CancellationToken>()).Returns(false);
        
        var result = await CreateHandler().HandleAsync(new CreateRoomCommand("general"), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("general");
        _rooms.Received(1).Add(Arg.Any<ChatRoom>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnConflict_WhenNameTaken()
    {
        _rooms.ExistsWithNameAsync(Arg.Any<RoomName>(), Arg.Any<CancellationToken>()).Returns(true);
        
        var result = await CreateHandler().HandleAsync(new CreateRoomCommand("general"), CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("room.name_taken");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}