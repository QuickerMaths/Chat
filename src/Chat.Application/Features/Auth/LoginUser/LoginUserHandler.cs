using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Application.Contracts;

namespace Chat.Application.Features.Auth.LoginUser;

public sealed class LoginUserHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        ITokenIssuer tokens
    )
{
    public async Task<Result<AccessTokenDto>> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken)
        => throw new NotImplementedException();
}