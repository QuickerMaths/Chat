using Chat.Domain.Common;

namespace Chat.Domain.ValueObjects;

public sealed record RoomName
{
    private RoomName(string value) => Value = value;
    public string Value { get; }

    public static RoomName Create(string input)
    {
        var trimmed = (input ?? string.Empty).Trim();

        if (trimmed.Length is < 3 or > 50)
        {
            throw new DomainException("room.name_invalid", "Room must be 3 to 50 characters");
        }

        return new RoomName(trimmed);
    }
    
    public override string ToString() => Value;
}