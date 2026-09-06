using AwesomeAssertions;
using Chat.Domain.Common;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.UnitTests.ValueObjects;

public sealed class RoomNameTests
{
    [Fact]
    public void Create_ShouldThrow_WhenBlank()
    {
        var act = () => RoomName.Create("   ");

        act.Should().Throw<DomainException>()
            .Which.Code.Should().Be("room.name_invalid");
    }

    [Fact]
    public void Create_ShouldThrow_WhenLongerThan50()
    {
        var act = () => RoomName.Create(new string('a', 51));

        act.Should().Throw<DomainException>()
            .Which.Code.Should().Be("room.name_invalid");
    }

    [Fact]
    public void Create_ShouldThrow_WhenShorterThan3()
    {
        var act = () => RoomName.Create("ge");
        
        act.Should().Throw<DomainException>()
            .Which.Code.Should().Be("room.name_invalid");
    }

    [Fact]
    public void Create_ShouldTrim_WhenSurroundedByWhitespace()
    {
        var act = RoomName.Create("   general ");
        
        act.Value.Should().Be("general");
    }
}