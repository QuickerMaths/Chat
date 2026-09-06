namespace Chat.Application.Contracts;

public sealed record AccessTokenDto(
        string AccessToken,
        DateTimeOffset ExpiresAtUtc,
        string UserName,
        Guid UserId
    );