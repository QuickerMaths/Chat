using Chat.Application.Abstractions;
using Chat.Application.Features.Auth.RegisterUser;
using Chat.Domain.Identifiers;
using Chat.Domain.Users;
using Chat.Domain.ValueObjects;

namespace Chat.Application.UnitTests.Features.Auth;

public sealed class RegisterUserHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
	private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
	private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
	private readonly FakeClock _clock = new();

	private RegisterUserHandler CreateHandler() => new(_users, _hasher, _unitOfWork, _clock);

    [Fact]
    public async Task HandleAsync_ShouldPersistHashedPassword_WhenUserNameIsFree()
    {
        _users.ExistsAsync(Arg.Any<UserName>(), Arg.Any<CancellationToken>()).Returns(false);
        _hasher.Hash("s3cret-passowrd").Returns("HASHED");
        var command = new RegisterUserCommand("alice", "s3cret-passowrd");
        
        var result = await CreateHandler().HandleAsync(command, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _users.Received(1).Add(Arg.Is<User>(u => u.PasswordHash == "HASHED"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnConflict_WhenUserNameTaken()
    {
        _users.ExistsAsync(Arg.Any<UserName>(), Arg.Any<CancellationToken>()).Returns(true);
        var command = new RegisterUserCommand("alice", "s3cret-passowrd");
        
        var result = await CreateHandler().HandleAsync(command, CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("user.name_taken");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}