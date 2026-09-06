using Chat.Domain.Common;

namespace Chat.Domain.ValueObjects;

public sealed record MessageBody
{
    private MessageBody(string value) => Value = value;
    public string Value { get; }
    
    public static MessageBody Create(string input)
    {
        var trimmed = (input ?? string.Empty).Trim();

        if (trimmed.Length is 0 or > 2000)
        {
            throw new DomainException("message.body_invalid", "Message body must be 1 to 2000 characters");
        }
        
        return new MessageBody(trimmed);
    }
    
    public override string ToString() => Value;
}