using Chat.Application.Abstractions;
using Chat.Application.Features.Rooms.JoinRoom;
using Chat.Domain.Identifiers;
using Chat.Domain.Rooms;
using Chat.Domain.ValueObjects;

namespace Chat.Application.UnitTests.Features.Rooms;

public sealed class JoinRoomHandlerTests
{
    private readonly IRoomRepository _rooms =  Substitute.For<IRoomRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly FakeClock _clock = new();
    private readonly UserId _caller = UserId.New();
    
    public JoinRoomHandlerTests() => _userContext.UserId.Returns(_caller);
    
    private JoinRoomHandler CreateHandler() => new(_rooms, _userContext, _unitOfWork, _clock);
    
    private ChatRoom RoomOwnedBy(UserId owner) 
     => ChatRoom.Create(RoomId.New(), RoomName.Create("general"), _caller, _clock.UtcNow);

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenRoomMissing()
    {
        _rooms.FindByIdAsync(Arg.Any<RoomId>(), Arg.Any<CancellationToken>()).Returns((ChatRoom?)null);
        
        var result = await CreateHandler().HandleAsync(new JoinRoomCommand(RoomId.New()), CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("room.not_found");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFalse_WhenAlreadyMember()
    {
        var room = RoomOwnedBy(_caller);
        
        var result = await CreateHandler().HandleAsync(new JoinRoomCommand(room.Id), CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeFalse();
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldSaveChanges_WhenNewlyJoined()
    {
        var room = RoomOwnedBy(UserId.New());
        _rooms.FindByIdAsync(room.Id, Arg.Any<CancellationToken>()).Returns(room);
        
        var result = await CreateHandler().HandleAsync(new JoinRoomCommand(room.Id), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}