using AwesomeAssertions;

using Chat.Domain.Common;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.UnitTests.ValueObjects;

public sealed class UserNameTests
{
    [Fact]
    public void Create_ShouldTrimAndLowercase_WhenInputHasMixedCase()
    {
        var userName = UserName.Create("  AliCe  ");

        userName.Value.Should().Be("alice");
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenShortedThen3()
    {
        var act = () => UserName.Create("av");

        act.Should().Throw<DomainException>()
            .Which.Code.Should().Be("user.name_too_short");
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenContainsIllegalCharacter()
    {
        var act = () => UserName.Create(@"illegal_name!?\");
        
        act.Should().Throw<DomainException>()
            .Which.Code.Should().Be("user.name_invalid");
    }

    [Fact]
    public void Equality_ShouldBeValueBased()
    {
        var a = UserName.Create("alice");
        var b = UserName.Create("ALICE");
        
        a.Should().Be(b);
    }
}