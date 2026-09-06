namespace Chat.Application.Features.Auth.LoginUser;

public sealed record LoginUserCommand(string UserName, string Password);