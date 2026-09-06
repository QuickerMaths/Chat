using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Contracts;
using Chat.Application.Features.Messages.GetMessages;
using Chat.Domain.Identifiers;
using Chat.Domain.Rooms;
using Chat.Domain.ValueObjects;

namespace Chat.Application.UnitTests.Features.Messages;

public sealed class GetMessagesHandlerTests
{
    private readonly IRoomRepository _rooms = Substitute.For<IRoomRepository>();
    private readonly IMessageQueries _messages = Substitute.For<IMessageQueries>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly UserId _caller = UserId.New();
    private readonly DateTimeOffset _now = new(2026, 9, 5, 12, 0,0, TimeSpan.Zero);

    public GetMessagesHandlerTests()
    {
        _userContext.UserId.Returns(_caller);
        _messages.FindPageAsync(Arg.Any<RoomId>(), Arg.Any<DateTimeOffset?>(), Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(new List<MessageDto>());
    }
    
    private GetMessagesHandler CreateHandler() => new GetMessagesHandler(_rooms, _messages, _userContext);

    private ChatRoom RoomWithMember(UserId member) =>
        ChatRoom.Create(RoomId.New(), RoomName.Create("general"), member, _now);

    [Fact]
    public async Task HandleAsync_ShouldClampLimitTo100_WhenLimitExceedsMaximum()
    {
        var room =  RoomWithMember(UserId.New());
        _rooms.FindByIdAsync(Arg.Any<RoomId>(), Arg.Any<CancellationToken>()).Returns(room);
        
        await CreateHandler().HandleAsync(new GetMessagesCommand(room.Id.Value, DateTimeOffset.UtcNow, 200), CancellationToken.None);
     
        await _messages.Received(1).FindPageAsync(Arg.Any<RoomId>(), Arg.Any<DateTimeOffset?>(), 100, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldDefaultLimitTo50_WhenLimitNotSupplied()
    {
        var room =  RoomWithMember(UserId.New());
        _rooms.FindByIdAsync(Arg.Any<RoomId>(), Arg.Any<CancellationToken>()).Returns(room);
        
        await CreateHandler().HandleAsync(new GetMessagesCommand(room.Id.Value, DateTimeOffset.UtcNow, null), CancellationToken.None);
        
        await _messages.Received(1).FindPageAsync(Arg.Any<RoomId>(), Arg.Any<DateTimeOffset?>(), 50, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenCallerIsNotMember()
    {
        var room =  RoomWithMember(UserId.New());
        _rooms.FindByIdAsync(Arg.Any<RoomId>(), Arg.Any<CancellationToken>()).Returns(room);
        
        var result = await CreateHandler().HandleAsync(new GetMessagesCommand(room.Id.Value, DateTimeOffset.UtcNow, null), CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("room.not_member");
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }
}