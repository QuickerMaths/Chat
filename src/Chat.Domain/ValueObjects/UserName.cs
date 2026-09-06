using System.Text.RegularExpressions;

using Chat.Domain.Common;

namespace Chat.Domain.ValueObjects;

public sealed partial record UserName
{
    private static readonly Regex AllowedPattern = MyRegex();
    private UserName(string value) => Value = value;
    public string Value { get; }

    public static UserName Create(string input)
    {
        var normalized = (input ?? string.Empty).Trim().ToLowerInvariant();

        if (normalized.Length < 3)
        {
            throw new DomainException("user.name_too_short", "Username must be at least 3 characters.");
        }

        if (normalized.Length > 32 || !AllowedPattern.IsMatch(normalized))
        {
            throw new DomainException("user.name_invalid", "Username may contain only a-z, 0-9, '.', '_', and '-', up to 43 characters.");
        }

        return new UserName(normalized);
    }
    
    public override string ToString() => Value;

    [GeneratedRegex("^[a-z0-9._-]+$")]
    private static partial Regex MyRegex();
}