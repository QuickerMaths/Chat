using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Features.Messages.SendMessage;
using Chat.Domain.Identifiers;
using Chat.Domain.Messages;
using Chat.Domain.Rooms;
using Chat.Domain.ValueObjects;

namespace Chat.Application.UnitTests.Features.Messages;

public sealed class SendMessageHandlerTests
{
    private readonly IRoomRepository _rooms = Substitute.For<IRoomRepository>();
    private readonly IMessageRepository _messages = Substitute.For<IMessageRepository>();
    private readonly IUserQueries _users = Substitute.For<IUserQueries>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly FakeClock _clock = new();
    private readonly UserId _caller = UserId.New();

    public SendMessageHandlerTests()
    {
        _userContext.UserId.Returns(_caller);
        _users.GetUserNameAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns("alice");
    }
 
    private SendMessageHandler CreateHandler() => new (_rooms, _messages, _users, _unitOfWork, _userContext, _clock);
    
    private ChatRoom RoomWithMember(UserId member)
     => ChatRoom.Create(RoomId.New(), RoomName.Create("general"), member, _clock.UtcNow);
    
    private ChatRoom RoomWithoutCaller() =>
    ChatRoom.Create(RoomId.New(), RoomName.Create("general"), UserId.New(), _clock.UtcNow);

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenRoomMissing()
    {
        _rooms.FindByIdAsync(Arg.Any<RoomId>(), Arg.Any<CancellationToken>()).Returns((ChatRoom?)null);
        
        var result = await CreateHandler().HandleAsync(new SendMessageCommand(Guid.NewGuid(), "hi", Guid.NewGuid()), CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("room.not_found");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenAuthorNotMember()
    {
        var room = RoomWithoutCaller();
        _rooms.FindByIdAsync(room.Id, Arg.Any<CancellationToken>()).Returns(room);
        
        var result = await CreateHandler().HandleAsync(new SendMessageCommand(room.Id.Value, "hi", Guid.NewGuid()), CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("room.forbidden");
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnExistingMessage_WhenClientMessageIdAlreadyStored()
    {
        var room = RoomWithMember(_caller);
        _rooms.FindByIdAsync(room.Id, Arg.Any<CancellationToken>()).Returns(room);
        
        var clientMessageId = Guid.NewGuid();
        var existing = Message.Send(room, _caller, MessageBody.Create("already sent"), clientMessageId, _clock.UtcNow);
        _messages.FindByClientIdAsync(Arg.Any<RoomId>(), clientMessageId,Arg.Any<CancellationToken>()).Returns(existing);

        var result = await CreateHandler().HandleAsync(new SendMessageCommand(room.Id.Value, "hi", clientMessageId), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Value!.Body.Should().Be("already sent");
        _messages.DidNotReceive().Add(Arg.Any<Message>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldStampSentAtFromClock_WhenSuccessful()
    {
        var room = RoomWithMember(_caller);
        _rooms.FindByIdAsync(room.Id, Arg.Any<CancellationToken>()).Returns(room);
        _messages.FindByClientIdAsync(Arg.Any<RoomId>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Message?)null);
        _clock.UtcNow = new DateTimeOffset(2026, 9, 5, 14, 30, 0, TimeSpan.Zero);
        
        var result = await CreateHandler().HandleAsync(new SendMessageCommand(room.Id.Value, "hi", Guid.NewGuid()), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Value!.SentAtUtc.Should().Be(_clock.UtcNow);
        _messages.Received(1).Add(Arg.Any<Message>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}