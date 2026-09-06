using System.Runtime.InteropServices.JavaScript;

using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Contracts;
using Chat.Domain.ValueObjects;

namespace Chat.Application.Features.Auth.LoginUser;

public sealed class LoginUserHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        ITokenIssuer tokens
    )
{
    public async Task<Result<AccessTokenDto>> HandleAsync(LoginUserCommand command, CancellationToken ct)
    {
        var invalidCredentials =
            ApplicationError.Validation("auth.invalid_credentials", "Invalid username or password.");

        var userName = UserName.Create(command.UserName);
        var user = await users.FindByUsernameAsync(userName, ct);

        return user is null || !hasher.Verify(user.PasswordHash, command.Password) 
            ? Result<AccessTokenDto>.Failure(invalidCredentials) 
            : Result<AccessTokenDto>.Success(tokens.Issue(user));
    }
}