using AwesomeAssertions;
using Chat.Domain.Common;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.UnitTests.ValueObjects;

public class MessageBodyTests
{
    [Fact]
    public void Create_ShouldThrow_WhenWhitespaceOnly()
    {
        var act = () => MessageBody.Create(" ");

        act.Should().Throw<DomainException>()
            .Which.Code.Should().Be("message.body_invalid");
    }

    [Fact]
    public void Create_ShouldThrow_WhenLongerThan2000()
    {
        var act = () => MessageBody.Create(new string('a', 2001));
        
        act.Should().Throw<DomainException>()
            .Which.Code.Should().Be("message.body_invalid");
    }

    [Fact]
    public void Create_ShouldThrow_WhenEmpty()
    {
        var act = () => MessageBody.Create(string.Empty);
        
        act.Should().Throw<DomainException>()
            .Which.Code.Should().Be("message.body_invalid");
    }

    [Fact]
    public void Create_ShouldPreserverInnerWhitespace()
    {
        var act = MessageBody.Create("  hello world  ");
        
        act.Value.Should().Be("hello world");
    }
}