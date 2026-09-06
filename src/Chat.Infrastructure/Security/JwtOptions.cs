using System.ComponentModel.DataAnnotations;

namespace Chat.Infrastructure.Security;

public sealed class JwtOptions
{
    [Required]
    public string Issuer { get; init; } = string.Empty;
    
    [Required]
    public string Audience { get; init; } = string.Empty;
    
    [Required]
    [MinLength(32)]
    public string Key { get; init; } = string.Empty;
 
    [Range(1, 1440)]
    public int AccessTokenLifetimeMinutes { get; init; } = 480;
}