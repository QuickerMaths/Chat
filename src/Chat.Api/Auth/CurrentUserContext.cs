using System.Security.Claims;

using Chat.Application.Abstractions;
using Chat.Domain.Identifiers;

namespace Chat.Api.Auth;

public sealed class CurrentUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public UserId UserId
    {
        get
        {
            var sub = httpContextAccessor.HttpContext?.User.FindFirstValue("sub")
                      ?? throw new InvalidOperationException("No authenticated user on the current request.");
            return new UserId(Guid.Parse(sub));
        }
    }
    
    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}