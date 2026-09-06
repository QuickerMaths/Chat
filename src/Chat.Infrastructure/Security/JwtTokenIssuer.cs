using System.Text;
using Chat.Application.Abstractions;
using Chat.Application.Contracts;
using Chat.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Chat.Infrastructure.Security;

internal sealed class JwtTokenIssuer(IOptions<JwtOptions> options, IClock clock) : ITokenIssuer
{
    private readonly JwtOptions _options = options.Value;

    public AccessTokenDto Issue(User user)
    {
        var issuedAt = clock.UtcNow;
        var expiresAt = issuedAt.AddMinutes(_options.AccessTokenLifetimeMinutes);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = issuedAt.UtcDateTime,
            NotBefore = issuedAt.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = credentials,
            Claims = new Dictionary<string, object>
            {
                ["sub"] = user.Id.Value.ToString(),
                ["name"] = user.UserName.Value
            }
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new AccessTokenDto(token, expiresAt, user.UserName.Value, user.Id.Value);
    }
}