using Chat.Application.Abstractions;
using Chat.Application.Common;
using Chat.Domain.Identifiers;
using Chat.Domain.Users;
using Chat.Domain.ValueObjects;

namespace Chat.Application.Features.Auth.RegisterUser;

public sealed class RegisterUserHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    IUnitOfWork unitOfWork,
    IClock clock
    )
{
    public async Task<Result<Guid>> HandleAsync(RegisterUserCommand command, CancellationToken ct)
    {
        var userName = UserName.Create(command.UserName);

        if (await users.ExistsAsync(userName, ct))
            return Result<Guid>.Failure(ApplicationError.Conflict("user.name_taken",
                "That username is already taken."));
        
        var passwordHash = hasher.Hash(command.Password);
        var user = User.Register(UserId.New(), userName, passwordHash, clock.UtcNow);
        
        users.Add(user);
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result<Guid>.Success(user.Id.Value);
    }
}