using Chat.Application.Abstractions;
using Chat.Application.Common;

namespace Chat.Application.Features.Auth.RegisterUser;

public sealed class RegisterUserHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    IUnitOfWork unitOfWork,
    IClock clock
    )
{
    public async Task<Result<Guid>> HandleAsync(RegisterUserCommand command, CancellationToken ct)
        => throw new NotImplementedException();
}