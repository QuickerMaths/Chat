using Chat.Application.Features.Auth.LoginUser;
using Chat.Application.Features.Auth.RegisterUser;

namespace Chat.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        // POST /api/auth/register => 201 { userId } / 409 (user.name_taken)
        group.MapPost("/register",
            async (RegisterUserCommand command, RegisterUserHandler handler, CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(command, ct);
                return result.ToHttpResult(userId =>
                    Results.Json(new { userId }, statusCode: StatusCodes.Status201Created)
                );
            });

        // POST /api/auth/login => 200 AccessTokenDto / 400 (auth.invalid_credentials)
        group.MapPost("/login",
            async (LoginUserCommand command, LoginUserHandler handler, CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(command, ct);
                return result.ToHttpResult(Results.Ok);
            });
    }
}