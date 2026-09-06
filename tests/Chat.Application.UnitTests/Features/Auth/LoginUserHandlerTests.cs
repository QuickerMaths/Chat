using Chat.Application.Abstractions;
using Chat.Application.Contracts;
using Chat.Application.Features.Auth.LoginUser;
using Chat.Domain.Identifiers;
using Chat.Domain.Users;
using Chat.Domain.ValueObjects;

namespace Chat.Application.UnitTests.Features.Auth;

public sealed class LoginUserHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenIssuer _tokens = Substitute.For<ITokenIssuer>();
    
    private LoginUserHandler CreateHandler() => new (_users, _hasher, _tokens);
    
    private static User CreateUser() =>
        User.Register(UserId.New(), UserName.Create("alice"), "HASHED", new DateTimeOffset(2026, 9, 5, 13, 0, 0, TimeSpan.Zero));

    [Fact]
    public async Task HandleAsync_ShouldReturnToken_WhenCredentialsValid()
    {
        var user = CreateUser();
        _users.FindByUsernameAsync(Arg.Any<UserName>(), Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify("HASHED", "correct-password").Returns(true);
        var token = new AccessTokenDto("jwt-token", new DateTimeOffset(2026, 9, 5, 13, 0, 0, TimeSpan.Zero), "alice", user.Id.Value);
        _tokens.Issue(user).Returns(token);
        
        var result = await CreateHandler().HandleAsync(new LoginUserCommand("alice", "correct-password"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("jwt-token");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidationError_WhenPasswordWrong()
    {
        var user = CreateUser();
        _users.FindByUsernameAsync(Arg.Any<UserName>(), Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify("HASHED", "correct-password").Returns(false);
        var handler = CreateHandler();

        var wrongPassword = await handler
            .HandleAsync(new LoginUserCommand("alice", "wrong-password"), CancellationToken.None);

        _users.FindByUsernameAsync(Arg.Any<UserName>(), Arg.Any<CancellationToken>()).Returns((User?)null);
        var unknownUser = await handler.HandleAsync(new LoginUserCommand("nobody", "wrong-password"), CancellationToken.None);
        
        wrongPassword.Error.Should().Be(unknownUser.Error);
        wrongPassword.Error!.Code.Should().Be("auth.invalid_credentials");
    }
}