using Chat.Application.Abstractions;

namespace Chat.Application.UnitTests;

public sealed class FakeClock : IClock
{
    public DateTimeOffset UtcNow { get; set; } = new(2026, 9, 5, 12, 0, 0 , TimeSpan.Zero);
}